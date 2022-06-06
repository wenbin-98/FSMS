using FSMS.Models;
using MySql.Data.MySqlClient;

namespace FSMS.Services
{
    public interface IDeliveryOrderService
    {
        Task<IEnumerable<DeliveryOrderListViewModel>> GetAllDeliveryOrderAsync();
        Task<DeliveryOrderDetailServiceModel> GetDeliveryOrderDetail(int id);
        Task CreateDeliveryOrderAsync(AddDeliveryOrderServiceModel serviceModel);
        Task UpdateDeliveryOrderAsync(EditDeliveryOrderServiceModel serviceModel);
        Task DeleteDeliveryOrderAsync(int id);
        Task<int> GetLastestDeliveryOrderSerialNumberAsync();
        Task ChangeDeliveryOrderStatusAsync(int id);
    }
    public class DeliveryOrderService : IDeliveryOrderService
    {
        private string ConnectionString { get; set; }
        private readonly IConfiguration _configuration;

        public DeliveryOrderService(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<DeliveryOrderListViewModel>> GetAllDeliveryOrderAsync()
        {
            List<DeliveryOrderListViewModel> deliveryOrders = new List<DeliveryOrderListViewModel>();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT d.id, d.serial_number, d.date, d.status, c.name FROM delivery_order d INNER JOIN customer c ON d.customer_id = c.id");

                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                    return deliveryOrders.AsEnumerable();
                }

                while (await dr.ReadAsync())
                {
                    deliveryOrders.Add(new DeliveryOrderListViewModel
                    {
                        SerialNumber = (int)dr["serial_number"],
                        Id = int.Parse(dr["id"].ToString()),
                        Date = DateTime.Parse(dr["date"].ToString()),
                        Status = bool.Parse(dr["status"].ToString()),
                        CustomerName = dr["name"].ToString()
                    });
                }
                conn.Close();
            }
            return deliveryOrders.AsEnumerable();
        }

        public async Task<DeliveryOrderDetailServiceModel> GetDeliveryOrderDetail(int id)
        {
            DeliveryOrderDetailServiceModel deliveryOrderDetail = new DeliveryOrderDetailServiceModel();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT d.*, c.* FROM delivery_order d INNER JOIN customer c ON d.customer_id = c.id WHERE d.id = {0}", id);
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                }
                while (await dr.ReadAsync())
                {
                    deliveryOrderDetail.Id = id;
                    deliveryOrderDetail.SerialNo = (int)dr["serial_number"];
                    deliveryOrderDetail.Date = DateTime.Parse(dr["date"].ToString());
                    deliveryOrderDetail.Status = bool.Parse(dr["status"].ToString());
                    deliveryOrderDetail.PurchaseOrder = dr["purchase_order"].ToString();
                    deliveryOrderDetail.Customer = new DocumentCustomerDetailViewModel(int.Parse(dr["customer_id"].ToString()), dr["name"].ToString(), dr["address1"].ToString(), dr["address2"].ToString(), dr["postcode"].ToString(), dr["city"].ToString(), dr["state"].ToString(), dr["phone"].ToString(), dr["email"].ToString());
                }
                dr.Close();

                cmdString = String.Format("SELECT ip.quantity inv_q, ip.price inv_p, ip.stock_id, s.* FROM ((delivery_order d INNER JOIN delivery_order_product ip ON ip.delivery_order_id = d.id ) INNER JOIN stock s ON s.id = ip.stock_id) WHERE d.id = {0}", id);
                cmd.CommandText = cmdString;
                dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                    return deliveryOrderDetail;
                }

                List<DeliveryOrderStocksDetails> stocks = new List<DeliveryOrderStocksDetails>();

                while (await dr.ReadAsync())
                {
                    stocks.Add(new DeliveryOrderStocksDetails
                    {
                        Id = (int)dr["stock_id"],
                        Description = dr["description"].ToString(),
                        Name = dr["name"].ToString(),
                        Quantity = (int)dr["inv_q"],
                    });
                }

                deliveryOrderDetail.Stocks = stocks;
                conn.Close();
            }
            return deliveryOrderDetail;
        }
        public async Task CreateDeliveryOrderAsync(AddDeliveryOrderServiceModel serviceModel)
        {
            int deliveryOrderId;
            string table = "delivery_order";
            string field = "serial_number, date, customer_id, purchase_order, status";
            string cmdString = String.Format("INSERT INTO {0}({1})  VALUES ({2}, '{3}', {4}, '{5}', {6})", table, field, serviceModel.SerialNo, serviceModel.Date.Date.ToString("yyyy-MM-dd"), serviceModel.CustomerId, serviceModel.PurchaseOrder, 0);
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);

                var dataReader = await cmd.ExecuteReaderAsync();

                deliveryOrderId = int.Parse(cmd.LastInsertedId.ToString());
                conn.Close();
                if (serviceModel.Stocks != null)
                {
                    var stock = serviceModel.Stocks;
                    table = "delivery_order_product";
                    field = "delivery_order_id, stock_id, quantity";

                    for (int i = 0; i < stock.Count; i++)
                    {
                        await conn.OpenAsync();
                        cmdString = String.Format("INSERT INTO {0}({1}) VALUES ({2}, {3}, {4})", table, field, deliveryOrderId, stock[i].Id, stock[i].Quantity);
                        cmd.CommandText = cmdString;
                        await cmd.ExecuteReaderAsync();
                        conn.Close();
                    }
                }
            }
        }
        public async Task UpdateDeliveryOrderAsync(EditDeliveryOrderServiceModel serviceModel)
        {
            string table = "delivery_order";
            string cmdString = String.Format("UPDATE {0} SET serial_number = '{1}', date = '{2}', customer_id = {3}, purchase_order = '{4}' WHERE id = {5}", table, serviceModel.SerialNo, serviceModel.Date.Date.ToString("yyyy-MM-dd"), serviceModel.CustomerId, serviceModel.PurchaseOrder, serviceModel.Id);
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);

                var dataReader = await cmd.ExecuteReaderAsync();

                conn.Close();

                if (serviceModel.Stocks != null)
                {
                    var stocks = serviceModel.Stocks;
                    table = "delivery_order_product";
                    string field = "delivery_order_id, stock_id, quantity";

                    //Dispose All Old Record
                    await conn.OpenAsync();
                    cmdString = String.Format("DELETE FROM delivery_order_product WHERE delivery_order_id = {0}", serviceModel.Id);
                    cmd.CommandText = cmdString;
                    await cmd.ExecuteReaderAsync();
                    conn.Close();

                    //Add the new product
                    table = "delivery_order_product";
                    field = "delivery_order_id, stock_id, quantity";

                    for (int i = 0; i < stocks.Count; i++)
                    {
                        await conn.OpenAsync();
                        cmdString = String.Format("INSERT INTO {0}({1})  VALUES ({2}, {3}, {4})", table, field, serviceModel.Id, stocks[i].Id, stocks[i].Quantity);
                        cmd.CommandText = cmdString;
                        await cmd.ExecuteReaderAsync();
                        conn.Close();
                    }
                }
                conn.Close();
            }
        }
        public async Task DeleteDeliveryOrderAsync(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("DELETE FROM delivery_order WHERE id = {0}", id);

                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                await cmd.ExecuteReaderAsync();

                conn.Close();
            }
        }
        public async Task<int> GetLastestDeliveryOrderSerialNumberAsync()
        {
            int serialNumber = 1;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT MAX(serial_number) number FROM delivery_order");
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                    return serialNumber;
                }

                while (await dr.ReadAsync())
                {
                    if (dr["number"].ToString() == String.Empty)
                        return serialNumber;

                    serialNumber = int.Parse(dr["number"].ToString()) + 1;
                }

                dr.Close();
                conn.Close();
            }
            return serialNumber;
        }
        public async Task ChangeDeliveryOrderStatusAsync(int id)
        {
            bool status = false;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT status FROM delivery_order WHERE id = {0}", id);
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                while (await dr.ReadAsync())
                {
                    status = (bool)dr["status"];
                }
                dr.Close();
                await conn.CloseAsync();

                if (status)
                {
                    status = false;
                }
                else
                {
                    status = true;
                }

                await conn.OpenAsync();
                cmdString = String.Format("UPDATE delivery_order SET status = {0} WHERE id = {1}", status, id);
                cmd.CommandText = cmdString;
                await cmd.ExecuteReaderAsync();

                conn.Close();
            }
        }
    }
}

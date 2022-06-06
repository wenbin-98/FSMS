using FSMS.Models;
using MySql.Data.MySqlClient;
using System.Globalization;

namespace FSMS.Services
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceListViewModel>> GetAllInvoiceAsync();
        Task<InvoiceDetailServiceModel> GetInvoiceDetail(int id);
        Task CreateInvoiceAsync(AddInvoiceServiceModel serviceModel);
        Task UpdateInvoiceAsync(EditInvoiceServiceModel serviceModel);
        Task DeleteInvoiceAsync(int id);
        Task<int> GetLastestInvoiceSerialNumberAsync();
        Task ChangePaymentStatusAsync(int id);
    }

    public class InvoiceService : IInvoiceService
    {
        private string ConnectionString { get; set; }
        private readonly IConfiguration _configuration;

        public InvoiceService(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<InvoiceListViewModel>> GetAllInvoiceAsync()
        {
            List<InvoiceListViewModel> invoices = new List<InvoiceListViewModel>();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT i.id, i.serial_number, i.date, i.status, c.name FROM invoice i INNER JOIN customer c ON i.customer_id = c.id");

                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                    return invoices.AsEnumerable();
                }

                while (await dr.ReadAsync())
                {
                    invoices.Add(new InvoiceListViewModel
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
            return invoices.AsEnumerable();
        }

        public async Task<InvoiceDetailServiceModel> GetInvoiceDetail(int id)
        {
            InvoiceDetailServiceModel invoiceDetail = new InvoiceDetailServiceModel();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT i.*, c.* FROM invoice i INNER JOIN customer c ON i.customer_id = c.id WHERE i.id = {0}", id);
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
                    invoiceDetail.Id = id;
                    invoiceDetail.SerialNo = (int)dr["serial_number"];
                    invoiceDetail.Date = DateTime.Parse(dr["date"].ToString());
                    invoiceDetail.DueDate = DateTime.Parse(dr["due_date"].ToString());
                    invoiceDetail.Subtotal = double.Parse(dr["subtotal"].ToString());
                    invoiceDetail.Tax = double.Parse(dr["tax"].ToString());
                    invoiceDetail.ShippingFee = double.Parse(dr["shipping_fee"].ToString());
                    invoiceDetail.Price = double.Parse(dr["total"].ToString());
                    invoiceDetail.PaymentStatus = bool.Parse(dr["status"].ToString());
                    invoiceDetail.PurchaseOrder = dr["purchase_order"].ToString();
                    invoiceDetail.Customer = new DocumentCustomerDetailViewModel(int.Parse(dr["customer_id"].ToString()), dr["name"].ToString(), dr["address1"].ToString(), dr["address2"].ToString(), dr["postcode"].ToString(), dr["city"].ToString(), dr["state"].ToString(), dr["phone"].ToString(), dr["email"].ToString());
                }
                dr.Close();

                cmdString = String.Format("SELECT ip.quantity inv_q, ip.price inv_p, ip.stock_id, s.* FROM ((invoice i INNER JOIN invoice_product ip ON ip.invoice_id = i.id ) INNER JOIN stock s ON s.id = ip.stock_id) WHERE i.id = {0}", id);
                cmd.CommandText = cmdString;
                dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                }

                if (dr.IsClosed) {
                    conn.Close();
                    return invoiceDetail;
                }

                List<InvoiceStocksDetails> stocks = new List<InvoiceStocksDetails>();

                while (await dr.ReadAsync())
                {
                    stocks.Add(new InvoiceStocksDetails
                    {
                        Id = (int)dr["stock_id"],
                        Description = dr["description"].ToString(),
                        Name = dr["name"].ToString(),
                        Quantity = (int)dr["inv_q"],
                        UnitPrice = double.Parse(dr["inv_p"].ToString()),
                        PictureUrl = dr["pic_url"].ToString()
                    });
                }

                invoiceDetail.Stocks = stocks;
                conn.Close();
            }
            return invoiceDetail;
        }
        public async Task CreateInvoiceAsync(AddInvoiceServiceModel serviceModel)
        {
            int invoiceId;
            string table = "invoice";
            string field = "serial_number, date, due_date, subtotal, tax, shipping_fee, total, customer_id, purchase_order, status";
            string cmdString = String.Format("INSERT INTO {0}({1})  VALUES ('{2}', '{3}', '{4}', {5}, {6}, {7}, {8}, {9}, '{10}', {11})", table, field, serviceModel.SerialNo, serviceModel.Date.Date.ToString("yyyy-MM-dd"), serviceModel.DueDate.Date.ToString("yyyy-MM-dd"), serviceModel.Subtotal, serviceModel.Tax, serviceModel.ShippingFee, serviceModel.Price, serviceModel.CustomerId, serviceModel.PurchaseOrder, 0);
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);

                var dataReader = await cmd.ExecuteReaderAsync();

                invoiceId = int.Parse(cmd.LastInsertedId.ToString());
                conn.Close();
                if (serviceModel.Stocks != null)
                {
                    var stock = serviceModel.Stocks;
                    table = "invoice_product";
                    field = "invoice_id, stock_id, quantity, price";

                    for (int i = 0; i < stock.Count; i++)
                    {
                        await conn.OpenAsync();
                        cmdString = String.Format("INSERT INTO {0}({1})  VALUES ({2}, {3}, {4}, {5})", table, field, invoiceId, stock[i].Id, stock[i].Quantity, stock[i].UnitPrice);
                        cmd.CommandText = cmdString;
                        await cmd.ExecuteReaderAsync();
                        conn.Close();

                        await conn.OpenAsync();
                        cmdString = String.Format("UPDATE stock SET quantity = (quantity - {0}) WHERE id = {1}", stock[i].Quantity, stock[i].Id);
                        cmd.CommandText = cmdString;
                        await cmd.ExecuteReaderAsync();
                        conn.Close();
                    }
                }
            }
        }
        public async Task UpdateInvoiceAsync(EditInvoiceServiceModel serviceModel)
        {
            string table = "invoice";
            string cmdString = String.Format("UPDATE {0} SET serial_number = '{1}', date = '{2}', due_date = '{3}', subtotal = {4}, tax = {5}, shipping_fee = {6}, total = {7}, customer_id = {8}, purchase_order = '{9}' WHERE id = {10}", table, serviceModel.SerialNo, serviceModel.Date.Date.ToString("yyyy-MM-dd"), serviceModel.DueDate.Date.ToString("yyyy-MM-dd"), serviceModel.Subtotal, serviceModel.Tax, serviceModel.ShippingFee, serviceModel.Price, serviceModel.CustomerId, serviceModel.PurchaseOrder, serviceModel.Id);
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);

                var dataReader = await cmd.ExecuteReaderAsync();

                conn.Close();

                if (serviceModel.Stocks != null)
                {
                    var stocks = serviceModel.Stocks;
                    table = "invoice_product";
                    string field = "invoice_id, stock_id, quantity, price";

                    //Get Existing Invoice Product
                    List<Tuple<double, int>> values = new List<Tuple<double, int>>();
                    await conn.OpenAsync();
                    cmdString = String.Format("SELECT quantity, stock_id FROM invoice_product WHERE invoice_id = {0}", serviceModel.Id);
                    cmd.CommandText = cmdString;
                    MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                    if (dr.HasRows)
                    {
                        while (await dr.ReadAsync())
                        {
                            values.Add(
                                new Tuple<double, int>(double.Parse(dr["quantity"].ToString()), (int)dr["stock_id"])
                            );
                        }
                        dr.Close();
                        conn.Close();

                        //Restore back the quantity
                        for (int j = 0; j < values.Count; j++)
                        {
                            await conn.OpenAsync();
                            cmdString = String.Format("UPDATE stock SET quantity = (quantity + {0}) WHERE id = {1}", values[j].Item1, values[j].Item2);
                            cmd.CommandText = cmdString;
                            await cmd.ExecuteReaderAsync();
                            conn.Close();
                        }

                        //Dispose All Old Record
                        await conn.OpenAsync();
                        cmdString = String.Format("DELETE FROM invoice_product WHERE invoice_id = {0}", serviceModel.Id);
                        cmd.CommandText = cmdString;
                        await cmd.ExecuteReaderAsync();
                        conn.Close();
                    }
                    else
                    {
                        dr.Close();
                        conn.Close();

                        //Dispose All Old Record
                        await conn.OpenAsync();
                        cmdString = String.Format("DELETE FROM invoice_product WHERE invoice_id = {0}", serviceModel.Id);
                        cmd.CommandText = cmdString;
                        await cmd.ExecuteReaderAsync();
                        conn.Close();
                    }

                    table = "invoice_product";
                    field = "invoice_id, stock_id, quantity, price";

                    for (int i = 0; i < stocks.Count; i++)
                    {
                        await conn.OpenAsync();
                        cmdString = String.Format("INSERT INTO {0}({1})  VALUES ({2}, {3}, {4}, {5})", table, field, serviceModel.Id, stocks[i].Id, stocks[i].Quantity, stocks[i].UnitPrice);
                        cmd.CommandText = cmdString;
                        await cmd.ExecuteReaderAsync();
                        conn.Close();

                        await conn.OpenAsync();
                        cmdString = String.Format("UPDATE stock SET quantity = (quantity - {0}) WHERE id = {1}", stocks[i].Quantity, stocks[i].Id);
                        cmd.CommandText = cmdString;
                        await cmd.ExecuteReaderAsync();
                        await conn.CloseAsync();
                    }
                }
                conn.Close();
            }
        }
        public async Task DeleteInvoiceAsync(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                //Get Existing Invoice Product
                List<Tuple<double, int>> values = new List<Tuple<double, int>>();
                await conn.OpenAsync();
                string cmdString = String.Format("SELECT quantity, stock_id FROM invoice_product WHERE invoice_id = {0}", id);
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                if (dr.HasRows)
                {
                    while (await dr.ReadAsync())
                    {
                        values.Add(
                            new Tuple<double, int>(double.Parse(dr["quantity"].ToString()), (int)dr["stock_id"])
                        );
                    }
                    dr.Close();
                    conn.Close();

                    //Restore back the quantity
                    for (int j = 0; j < values.Count; j++)
                    {
                        await conn.OpenAsync();
                        cmdString = String.Format("UPDATE stock SET quantity = (quantity + {0}) WHERE id = {1}", values[j].Item1, values[j].Item2);
                        cmd.CommandText = cmdString;
                        await cmd.ExecuteReaderAsync();
                        conn.Close();
                    }
                }
                else
                {
                    dr.Close();
                    conn.Close();
                }

                cmdString = String.Format("DELETE FROM invoice WHERE id = {0}", id);

                await conn.OpenAsync();
                cmd.CommandText = cmdString;
                await cmd.ExecuteReaderAsync();

                conn.Close();
            }
        }
        public async Task<int> GetLastestInvoiceSerialNumberAsync()
        {
            int serialNumber = 1;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT MAX(serial_number) number FROM invoice");
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
        public async Task ChangePaymentStatusAsync(int id)
        {
            bool status = false;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT status FROM invoice WHERE id = {0}", id);
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
                cmdString = String.Format("UPDATE invoice SET status = {0} WHERE id = {1}", status, id);
                cmd.CommandText = cmdString;
                await cmd.ExecuteReaderAsync();

                conn.Close();
            }
        }
    }
}

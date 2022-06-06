using FSMS.Models;
using MySql.Data.MySqlClient;

namespace FSMS.Services
{
    public interface IQuotationService
    {
        Task<IEnumerable<QuotationListViewModel>> GetAllQuotationAsync();
        Task<QuotationDetailServiceModel> GetQuotationDetail(int id);
        Task CreateQuotationAsync(AddQuotationServiceModel serviceModel);
        Task UpdateQuotationAsync(EditQuotationServiceModel serviceModel);
        Task DeleteQuotationAsync(int id);
        Task<int> GetLastestQuotationSerialNumberAsync();
        Task ChangeQuotationStatusAsync(int id);
    }
    public class QuotationService : IQuotationService
    {
        private string ConnectionString { get; set; }
        private readonly IConfiguration _configuration;

        public QuotationService(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<QuotationListViewModel>> GetAllQuotationAsync()
        {
            List<QuotationListViewModel> quotations = new List<QuotationListViewModel>();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT q.id, q.serial_number, q.date, q.status, c.name FROM quotation q INNER JOIN customer c ON q.customer_id = c.id");

                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                    return quotations.AsEnumerable();
                }

                while (await dr.ReadAsync())
                {
                    quotations.Add(new QuotationListViewModel
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
            return quotations.AsEnumerable();
        }
        public async Task<QuotationDetailServiceModel> GetQuotationDetail(int id)
        {
            QuotationDetailServiceModel quotationDetail = new QuotationDetailServiceModel();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT q.*, c.* FROM quotation q INNER JOIN customer c ON q.customer_id = c.id WHERE q.id = {0}", id);
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
                    quotationDetail.Id = id;
                    quotationDetail.SerialNo = (int)dr["serial_number"];
                    quotationDetail.Date = DateTime.Parse(dr["date"].ToString());
                    quotationDetail.DueDate = DateTime.Parse(dr["due_date"].ToString());
                    quotationDetail.Subtotal = double.Parse(dr["subtotal"].ToString());
                    quotationDetail.Tax = double.Parse(dr["tax"].ToString());
                    quotationDetail.ShippingFee = double.Parse(dr["shipping_fee"].ToString());
                    quotationDetail.Price = double.Parse(dr["total"].ToString());
                    quotationDetail.Status = bool.Parse(dr["status"].ToString());
                    quotationDetail.Customer = new DocumentCustomerDetailViewModel(int.Parse(dr["customer_id"].ToString()), dr["name"].ToString(), dr["address1"].ToString(), dr["address2"].ToString(), dr["postcode"].ToString(), dr["city"].ToString(), dr["state"].ToString(), dr["phone"].ToString(), dr["email"].ToString());
                }
                dr.Close();

                cmdString = String.Format("SELECT qp.quantity quo_q, qp.price quo_p, qp.stock_id, s.* FROM ((quotation q INNER JOIN quotation_product qp ON qp.quotation_id = q.id ) INNER JOIN stock s ON s.id = qp.stock_id) WHERE q.id = {0}", id);
                cmd.CommandText = cmdString;
                dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                }

                if (dr.IsClosed)
                {
                    conn.Close();
                    return quotationDetail;
                }

                List<QuotationStocksDetails> stocks = new List<QuotationStocksDetails>();

                while (await dr.ReadAsync())
                {
                    stocks.Add(new QuotationStocksDetails
                    {
                        Id = (int)dr["stock_id"],
                        Description = dr["description"].ToString(),
                        Name = dr["name"].ToString(),
                        Quantity = (int)dr["quo_q"],
                        UnitPrice = double.Parse(dr["quo_p"].ToString()),
                        PictureUrl = dr["pic_url"].ToString()
                    });
                }

                quotationDetail.Stocks = stocks;
                conn.Close();
            }
            return quotationDetail;
        }
        public async Task CreateQuotationAsync(AddQuotationServiceModel serviceModel)
        {
            int quotationId;
            string table = "quotation";
            string field = "serial_number, date, due_date, subtotal, tax, shipping_fee, total, customer_id, status";
            string cmdString = String.Format("INSERT INTO {0}({1})  VALUES ('{2}', '{3}', '{4}', {5}, {6}, {7}, {8}, {9}, {10})", table, field, serviceModel.SerialNo, serviceModel.Date.ToString("yyyy-MM-dd"), serviceModel.DueDate.ToString("yyyy-MM-dd"), serviceModel.Subtotal, serviceModel.Tax, serviceModel.ShippingFee, serviceModel.Price, serviceModel.CustomerId, 0);
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);

                await cmd.ExecuteReaderAsync();

                quotationId = int.Parse(cmd.LastInsertedId.ToString());
                conn.Close();
                if (serviceModel.Stocks != null)
                {
                    var stock = serviceModel.Stocks;
                    table = "quotation_product";
                    field = "quotation_id, stock_id, quantity, price";

                    for (int i = 0; i < stock.Count; i++)
                    {
                        await conn.OpenAsync();
                        cmdString = String.Format("INSERT INTO {0}({1}) VALUES ({2}, {3}, {4}, {5})", table, field, quotationId, stock[i].Id, stock[i].Quantity, stock[i].UnitPrice);
                        cmd.CommandText = cmdString;
                        await cmd.ExecuteReaderAsync();
                        conn.Close();
                    }
                }
            }
        }
        public async Task UpdateQuotationAsync(EditQuotationServiceModel serviceModel)
        {
            string table = "quotation";
            string cmdString = String.Format("UPDATE {0} SET serial_number = '{1}', date = '{2}', due_date = '{3}', subtotal = {4}, tax = {5}, shipping_fee = {6}, total = {7}, customer_id = {8} WHERE id = {9}", table, serviceModel.SerialNo, serviceModel.Date.Date.ToString("yyyy-MM-dd"), serviceModel.DueDate.Date.ToString("yyyy-MM-dd"), serviceModel.Subtotal, serviceModel.Tax, serviceModel.ShippingFee, serviceModel.Price, serviceModel.CustomerId, serviceModel.Id);
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);

                await cmd.ExecuteReaderAsync();

                conn.Close();

                if (serviceModel.Stocks != null)
                {
                    var stocks = serviceModel.Stocks;
                    table = "quotation_product";
                    string field = "quotation_id, stock_id, quantity, price";

                    //Dispose All Old Record
                    await conn.OpenAsync();
                    cmdString = String.Format("DELETE FROM quotation_product WHERE quotation_id = {0}", serviceModel.Id);
                    cmd.CommandText = cmdString;
                    await cmd.ExecuteReaderAsync();
                    conn.Close();

                    //Add the new product
                    table = "quotation_product";
                    field = "quotation_id, stock_id, quantity, price";

                    for (int i = 0; i < stocks.Count; i++)
                    {
                        await conn.OpenAsync();
                        cmdString = String.Format("INSERT INTO {0}({1})  VALUES ({2}, {3}, {4}, {5})", table, field, serviceModel.Id, stocks[i].Id, stocks[i].Quantity, stocks[i].UnitPrice);
                        cmd.CommandText = cmdString;
                        await cmd.ExecuteReaderAsync();
                        conn.Close();
                    }
                }
                conn.Close();
            }
        }
        public async Task DeleteQuotationAsync(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("DELETE FROM quotation WHERE id = {0}", id);

                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                await cmd.ExecuteReaderAsync();

                conn.Close();
            }
        }
        public async Task<int> GetLastestQuotationSerialNumberAsync()
        {
            int serialNumber = 1;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT MAX(serial_number) number FROM quotation");
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
        public async Task ChangeQuotationStatusAsync(int id)
        {
            bool status = false;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT status FROM quotation WHERE id = {0}", id);
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
                cmdString = String.Format("UPDATE quotation SET status = {0} WHERE id = {1}", status, id);
                cmd.CommandText = cmdString;
                await cmd.ExecuteReaderAsync();

                conn.Close();
            }
        }
    }
}

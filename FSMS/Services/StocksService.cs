using FSMS.Models;
using MySql.Data.MySqlClient;

namespace FSMS.Services
{
    public interface IStocksService
    {
        Task<IEnumerable<StocksListViewModel>> GetAllStocksAsync();
        Task CreateStockAsync(AddStocksServiceModel serviceModel);
        Task<StockDetalsServiceModel> GetStockDetailsAsync(int id);
        Task<List<StockDetalsServiceModel>> GetStockDropDownAsync(string filter);
        Task EditStockAsync(EditStockServiceModel serviceModel);
        Task DeleteStockAsync(int id);
    }

    public class StocksService : IStocksService
    {
        private string ConnectionString { get; set; }
        private readonly IConfiguration _configuration;

        public StocksService(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<StocksListViewModel>> GetAllStocksAsync()
        {
            List<StocksListViewModel> stocks = new List<StocksListViewModel>();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT id, name, quantity, price FROM stock");

                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                    throw new Exception("Product Not Found");
                }

                while (await dr.ReadAsync())
                {
                    stocks.Add(new StocksListViewModel
                    {
                        Id = int.Parse(dr["id"].ToString()),
                        Name = dr["name"].ToString(),
                        Quantity = int.Parse(dr["quantity"].ToString()),
                        Price = double.Parse(dr["price"].ToString())
                    });
                }
                conn.Close();
            }
            return stocks.AsEnumerable();
        }

        public async Task CreateStockAsync(AddStocksServiceModel serviceModel)
        {
            string table = "stock";
            string field = "name, description, price, quantity, pic_url";
            string cmdString = String.Format("INSERT INTO {0}({1}) VALUES ('{2}', '{3}', '{4}', '{5}', '{6}')", table, field, serviceModel.Name, serviceModel.Description, serviceModel.Price, serviceModel.Quantity, serviceModel.Picture);
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);

                var x = await cmd.ExecuteReaderAsync();
                conn.Close();
            }
        }

        public async Task<StockDetalsServiceModel> GetStockDetailsAsync(int id)
        {
            StockDetalsServiceModel stockDetailServiceModel = new StockDetalsServiceModel();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT * FROM stock WHERE id = {0}", id);
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                    throw new Exception("User Not Found");
                }

                while (await dr.ReadAsync())
                {
                    stockDetailServiceModel.Id = id;
                    stockDetailServiceModel.Name = dr["name"].ToString();
                    stockDetailServiceModel.Description = dr["description"].ToString();
                    stockDetailServiceModel.Price = double.Parse(dr["price"].ToString());
                    stockDetailServiceModel.Quantity = int.Parse(dr["quantity"].ToString());
                    stockDetailServiceModel.Picture = dr["pic_url"].ToString();
                }

                dr.Close();
                conn.Close();
            }

            return stockDetailServiceModel;
        }

        public async Task EditStockAsync(EditStockServiceModel serviceModel)
        {
            string table = "stock";
            string cmdString = null;
            if (serviceModel.Picture != String.Empty)
            {
                cmdString = String.Format("UPDATE {0} SET name = '{1}', description = '{2}', quantity = {3}, price = {4}, pic_url = '{5}' WHERE id = {6}", table, serviceModel.Name, serviceModel.Description, serviceModel.Quantity, serviceModel.Price, serviceModel.Picture, serviceModel.Id);
            }
            else
            {
                cmdString = String.Format("UPDATE {0} SET name = '{1}', description = '{2}', quantity = {3}, price = {4} WHERE id = {5}", table, serviceModel.Name, serviceModel.Description, serviceModel.Quantity, serviceModel.Price, serviceModel.Id);
            }

            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);

                var x = await cmd.ExecuteReaderAsync();
                conn.Close();
            }
        }

        public async Task<List<StockDetalsServiceModel>> GetStockDropDownAsync(string filter)
        {
            List<StockDetalsServiceModel> stocksServiceModels = new List<StockDetalsServiceModel>();
            string cmdString = String.Empty;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                if (filter == null)
                {
                    cmdString = String.Format("SELECT id, name, description, price FROM stock");
                }
                else
                {
                    cmdString = String.Format("SELECT id, name, description, price FROM stock WHERE name LIKE '%{0}%' OR description LIKE '%{0}%'", filter);
                }
                
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close(); 
                    return stocksServiceModels;
                }

                while (await dr.ReadAsync())
                {
                    stocksServiceModels.Add(new StockDetalsServiceModel
                    {
                        Id = int.Parse(dr["id"].ToString()),
                        Name = dr["name"].ToString(),
                        Description = dr["description"].ToString(),
                        Price = double.Parse(dr["price"].ToString())
                    });
                }

                dr.Close();
                conn.Close();
            }

            return stocksServiceModels;
        }

        public async Task DeleteStockAsync(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("DELETE FROM stock WHERE id = {0}", id);

                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                await cmd.ExecuteReaderAsync();

                conn.Close();
            }
        }
    }
}

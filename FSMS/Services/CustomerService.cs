using FSMS.Models;
using MySql.Data.MySqlClient;

namespace FSMS.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerListViewModel>> GetAllCustomerAsync();
        Task CreateCustomerAsync(AddCustomerServiceModel serviceModel);
        Task EditCustomerAsync(EditCustomerServiceModel serviceModel);
        Task<CustomerDetailServiceModel> GetCustomerDetailsAsync(int id);
        Task<CustomerDetailServiceModel> GetMerchantDetailsAsync();
        Task<List<CustomerDetailServiceModel>> GetCustomerDropDownAsync(string filter);
        Task DeleteCustomerAsync(int id);

    }

    public class CustomerService : ICustomerService
    {
        public string ConnectionString { get; set; }
        public IConfiguration _configuration;

        public CustomerService(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<CustomerListViewModel>> GetAllCustomerAsync()
        {
            List<CustomerListViewModel> customers = new List<CustomerListViewModel>();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT id, name, phone FROM customer WHERE merchant = 0");

                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();
                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                    return customers.AsEnumerable();
                }

                while (await dr.ReadAsync())
                {
                    customers.Add(new CustomerListViewModel
                    {
                        Id = int.Parse(dr["id"].ToString()),
                        Name = dr["name"].ToString(),
                        Phone = dr["phone"].ToString()
                    });
                }
                conn.Close();
            }
            return customers.AsEnumerable();
        }
        public async Task CreateCustomerAsync(AddCustomerServiceModel serviceModel)
        {
            string table = "customer";
            string field = "name, address1, address2, postcode, city, state, phone, email, merchant";
            string cmdString = String.Format("INSERT INTO {0}({1}) VALUES ('{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', {10})", table, field, serviceModel.Name, serviceModel.Address1, serviceModel.Address2, serviceModel.Postcode, serviceModel.City, serviceModel.State, serviceModel.Phone, serviceModel.Email, Byte.Parse("0"));
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);

                var x = await cmd.ExecuteReaderAsync();
                conn.Close();
            }
        }
        public async Task EditCustomerAsync(EditCustomerServiceModel serviceModel)
        {
            string table = "customer";
            string cmdString = String.Format("UPDATE {0} SET name = '{1}', address1 = '{2}', address2 = '{3}', postcode = '{4}', city = '{5}', state = '{6}', phone = '{7}', email = '{8}' WHERE id = {9}", table, serviceModel.Name, serviceModel.Address1, serviceModel.Address2, serviceModel.Postcode, serviceModel.City, serviceModel.State, serviceModel.Phone, serviceModel.Email, serviceModel.Id);
            
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);

                var x = await cmd.ExecuteReaderAsync();
                conn.Close();
            }
        }
        public async Task<CustomerDetailServiceModel> GetCustomerDetailsAsync(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT * FROM customer WHERE id = {0}", id);
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                    throw new Exception("User Not Found");
                }

                CustomerDetailServiceModel customerDetailsServiceModel = new CustomerDetailServiceModel();

                while (await dr.ReadAsync())
                {
                    customerDetailsServiceModel.Id = int.Parse(dr["id"].ToString());
                    customerDetailsServiceModel.Name = dr["name"].ToString();
                    customerDetailsServiceModel.Address1 = dr["address1"].ToString();
                    customerDetailsServiceModel.Address2 = dr["address2"].ToString();
                    customerDetailsServiceModel.Postcode = dr["postcode"].ToString();
                    customerDetailsServiceModel.City = dr["city"].ToString();
                    customerDetailsServiceModel.State = dr["state"].ToString();
                    customerDetailsServiceModel.Phone = dr["phone"].ToString();
                    customerDetailsServiceModel.Email = dr["email"].ToString();
                }

                dr.Close();
                conn.Close();
                return customerDetailsServiceModel;
            }
        }
        public async Task<CustomerDetailServiceModel> GetMerchantDetailsAsync()
        {
            CustomerDetailServiceModel customerDetailsServiceModel = new CustomerDetailServiceModel();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT * FROM customer WHERE merchant = 1");
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                    return customerDetailsServiceModel;
                }

                while (await dr.ReadAsync())
                {
                    customerDetailsServiceModel.Id = int.Parse(dr["id"].ToString());
                    customerDetailsServiceModel.Name = dr["name"].ToString();
                    customerDetailsServiceModel.Address1 = dr["address1"].ToString();
                    customerDetailsServiceModel.Address2 = dr["address2"].ToString();
                    customerDetailsServiceModel.Postcode = dr["postcode"].ToString();
                    customerDetailsServiceModel.City = dr["city"].ToString();
                    customerDetailsServiceModel.State = dr["state"].ToString();
                    customerDetailsServiceModel.Phone = dr["phone"].ToString();
                    customerDetailsServiceModel.Email = dr["email"].ToString();
                }

                dr.Close();
                conn.Close();
                return customerDetailsServiceModel;
            }
        }
        public async Task<List<CustomerDetailServiceModel>> GetCustomerDropDownAsync(string filter)
        {
            List<CustomerDetailServiceModel> customerServiceModels = new List<CustomerDetailServiceModel>();
            string cmdString = String.Empty;
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                if (filter == null)
                {
                    cmdString = String.Format("SELECT id, name FROM customer WHERE merchant = 0");
                }
                else
                {
                    cmdString = String.Format("SELECT id, name FROM customer WHERE name LIKE '%{0}%' AND merchant = 0", filter);
                }

                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                    return customerServiceModels;
                }

                while (await dr.ReadAsync())
                {
                    customerServiceModels.Add(new CustomerDetailServiceModel
                    {
                        Id = int.Parse(dr["id"].ToString()),
                        Name = dr["name"].ToString()
                    });
                }

                dr.Close();
                conn.Close();
            }

            return customerServiceModels;
        }
        public async Task DeleteCustomerAsync(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("DELETE FROM customer WHERE id = {0}", id);

                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                await cmd.ExecuteReaderAsync();

                conn.Close();
            }
        }
    }
}

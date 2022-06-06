using FSMS.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using MySql.Data.MySqlClient;
using BCryptNet = BCrypt.Net.BCrypt;

namespace FSMS.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<UserListViewModel>> GetAllUserAsync();
        Task<SignInServiceModel> LoginAccountAsync(string username, string password);
        Task<bool> CreateUserAsync(AddUserServiceModel serviceModel);
        Task<bool> EditUserAsync(EditUserServiceModel serviceModel);
        Task<UserDetailsServiceModel> GetUserDetailsAsync(int id);
        Task DeleteUserAsync(int id);
    }

    public class AccountService : IAccountService
    {
        private string ConnectionString { get; set; }
        private readonly IConfiguration _configuration;

        public AccountService(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<UserListViewModel>> GetAllUserAsync()
        {
            List<UserListViewModel> users = new List<UserListViewModel>();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT id, username, name FROM user");

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
                    users.Add(new UserListViewModel
                    {
                        Id = int.Parse(dr["id"].ToString()),
                        Name = dr["name"].ToString(),
                        Username = dr["username"].ToString()
                    });
                }
                conn.Close();
            }
            return users.AsEnumerable();
        }
        public async Task<SignInServiceModel> LoginAccountAsync(string username, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string currentPassword = null;
                bool verify;
                string name = null;
                string role = null;

                string cmdString = String.Format("SELECT username, password, name, role FROM user WHERE username = '{0}'", username);
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                    throw new Exception("Invalid Credential");
                }


                while (await dr.ReadAsync())
                {
                    currentPassword = dr["password"].ToString();
                    name = dr["name"].ToString();
                    role = dr["role"].ToString();
                }

                verify = BCryptNet.Verify(password, currentPassword);


                if (!verify)
                {
                    dr.Close();
                    conn.Close();
                    throw new Exception("Invalid Credential");
                }

                SignInServiceModel signInServiceModel = new SignInServiceModel(username, name, role);

                dr.Close();
                conn.Close();
                return signInServiceModel;
            }
        }
        public async Task<bool> CreateUserAsync(AddUserServiceModel serviceModel)
        {
            string table = "user";
            string field = "username, password, name, role, phone_no";
            serviceModel.Password = BCryptNet.HashPassword(serviceModel.Password);
            string cmdString = String.Format("INSERT INTO {0}({1}) VALUES ('{2}', '{3}', '{4}', '{5}', '{6}')", table, field, serviceModel.Username, serviceModel.Password, serviceModel.Name, serviceModel.Role, serviceModel.PhoneNo);
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(cmdString, conn);

                    var x = await cmd.ExecuteReaderAsync();
                    conn.Close();
                }catch (Exception ex)
                {
                    return false;
                }
                
                return true;
            }
        }
        public async Task<bool> EditUserAsync(EditUserServiceModel serviceModel)
        {
            string table = "user";
            string cmdString = null;
            if (serviceModel.Password != String.Empty)
            {
                serviceModel.Password = BCryptNet.HashPassword(serviceModel.Password);
                cmdString = String.Format("UPDATE {0} SET username = '{1}', password = '{2}', name = '{3}', role = '{4}', phone_no = '{5}' WHERE id = {6}", table, serviceModel.Username, serviceModel.Password, serviceModel.Name, serviceModel.Role, serviceModel.PhoneNo, serviceModel.Id);
            }
            else
            {
                cmdString = String.Format("UPDATE {0} SET username = '{1}', name = '{2}', role = '{3}', phone_no = '{4}' WHERE id = {5}", table, serviceModel.Username, serviceModel.Name, serviceModel.Role, serviceModel.PhoneNo, serviceModel.Id);
            }
            
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(cmdString, conn);

                    var x = await cmd.ExecuteReaderAsync();
                    conn.Close();
                }catch (Exception ex)
                {
                    return false;
                }
                
                return true;
            }
        }
        public async Task<UserDetailsServiceModel> GetUserDetailsAsync(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("SELECT * FROM user WHERE id = {0}", id);
                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                MySqlDataReader dr = (MySqlDataReader)await cmd.ExecuteReaderAsync();

                if (!dr.HasRows)
                {
                    dr.Close();
                    conn.Close();
                    throw new Exception("User Not Found");
                }

                UserDetailsServiceModel userDetailsServiceModel = new UserDetailsServiceModel();

                while (await dr.ReadAsync())
                {
                    userDetailsServiceModel.Name = dr["name"].ToString();
                    userDetailsServiceModel.PhoneNo = dr["phone_no"].ToString();
                    userDetailsServiceModel.Username = dr["username"].ToString();
                    userDetailsServiceModel.Role = dr["role"].ToString();
                    userDetailsServiceModel.Id = int.Parse(dr["id"].ToString());
                }

                dr.Close();
                conn.Close();
                return userDetailsServiceModel;
            }
        }
        public async Task DeleteUserAsync(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                string cmdString = String.Format("DELETE FROM user WHERE id = {0}", id);

                await conn.OpenAsync();
                MySqlCommand cmd = new MySqlCommand(cmdString, conn);
                await cmd.ExecuteReaderAsync();

                conn.Close();
            }
        }
    }
}

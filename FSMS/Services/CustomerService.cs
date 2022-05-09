using FSMS.Models;
using MySql.Data.MySqlClient;

namespace FSMS.Services
{
    public class CustomerService : ICustomerService
    {
        public string ConnectionString { get; set; }
        public IConfiguration _configuration;

        public CustomerService(IConfiguration configuration)
        {
            _configuration = configuration;
            ConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public List<TestEmployee> GetEmployees()
        {
            var employees = new List<TestEmployee>();
            using (MySqlConnection conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT employee_id, last_name FROM employees", conn);
                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    TestEmployee employee = new TestEmployee();
                    employee.Id = Convert.ToInt32(dr["employee_id"]);
                    employee.Name = dr["last_name"].ToString();
                    employees.Add(employee);
                }
                dr.Close();
                conn.Close();
            }
            return employees.ToList();
        }
    }
    public interface ICustomerService
    {
        public List<TestEmployee> GetEmployees();
    }
}

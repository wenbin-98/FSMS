namespace FSMS.Models
{
    public class SignInServiceModel
    {
        public string Username { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }

        public SignInServiceModel(string username, string name, string role)
        {
            Username = username;
            Name = name;
            Role = role;
        }
    }
}

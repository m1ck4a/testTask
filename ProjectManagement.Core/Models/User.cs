using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Core.Models
{
    public class User
    {
        public const int MAX_LOGIN_LENGTH = 28;
        
        public int Id { get; set; }

        public string? login {  get; set; }

        public string? password { get; set; }

        public string? roleName { get; set; }

        public User() { }

        private User(int id, string? login, string? password, string role)
        {
            Id = id;
            this.login = login;
            this.password = password;
            this.roleName = role;
        }

        private User(string? login, string? password, string role)
        {
            this.login = login;
            this.password = password;
            this.roleName = role;
        }

        public static async Task<User> Create(int id, string? login, string? password, string? roleName) 
        {
            if (string.IsNullOrWhiteSpace(login) || login.Length > MAX_LOGIN_LENGTH)
            {
                throw new ArgumentException($"Login can't be empty or more then {MAX_LOGIN_LENGTH} symbols");
            }

            if (string.IsNullOrWhiteSpace(roleName)) 
            {
                throw new ArgumentException($"RoleName can't be empty", nameof(roleName));
            }

            var User = new User(id, login, password, roleName);

            return User;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Core.Models
{
    public class Role
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Role() { }

        private Role( string? name)
        {
            Name = name;
        }

        public static async Task<Role> Create(string? name)
        {

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"RoleName can't be empty", nameof(name));
            }

            var Role = new Role(name);

            return Role;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Core.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public User User { get; set; }

        public Project() { }

        private Project(string name, string status, int userId, User user)
        {
            Name = name;
            Status = status;
            UserId = userId;
            User = user;
        }

        private Project(string name, string status)
        {
            Name = name;
            Status = status;    
        }


        public static async Task<Project> Create(string? name, string status)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"Name project can't be empty", nameof(name));
            }

            var Project = new Project(name, status);

            return Project;
        }

        public static async Task<Project> Create(string? name, string status, int userId, User user)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"Name project can't be empty", nameof(name));
            }

            var Project = new Project(name, status, userId, user);

            return Project;
        }
    }
}

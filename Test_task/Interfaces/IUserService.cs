using ProjectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_task.Interfaces
{
    public interface IUserService
    {
        Task<int> CreateUserAsync(User user, int createdByUserId);
        Task UpdateUserAsync(User user, int updatedByUserId);
        Task<User> GetUserByIdAsync(int id);
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserByLoginAsync(string? login);
    }
}

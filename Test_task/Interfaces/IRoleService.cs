using ProjectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_task.Interfaces
{
    public interface IRoleService
    {
        Task CreateRoleAsync(Role role, int createdByUserId);
        Task<Role> GetRoleByIdAsync(int id);
        Task<List<Role>> GetRolesAsync();
    }
}

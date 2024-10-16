using ProjectManagement.Core.Models;
using System.Net;

namespace ProjectManagement.Core.Abstractions
{
    public interface IRolesRepository
    {
        Task<Role> GetRoleById(int id);
        Task<List<Role>> GetAll();
        Task<int> Create(Role role, int createdByUserId);
    }
}
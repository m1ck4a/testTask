using ProjectManagement.Core.Abstractions;
using ProjectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test_task.Interfaces;

namespace Test_task.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRolesRepository _roleRepository;

        public RoleService(IRolesRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task CreateRoleAsync(Role role, int createdByUserId)
        {
            await _roleRepository.Create(role, createdByUserId);
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            return await _roleRepository.GetRoleById(id);
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await _roleRepository.GetAll();
        }
    }
}

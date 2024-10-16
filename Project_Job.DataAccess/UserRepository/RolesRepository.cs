using Microsoft.EntityFrameworkCore;
using Project_Job.DataAccess;
using ProjectManagement.Architecture.Mappers;
using ProjectManagement.Core.Abstractions;
using ProjectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Architecture.UserRepository
{
    public class RolesRepository : IRolesRepository
    {
        private readonly ProjectManagementDbContext _context;
        private readonly RoleMapper _roleMapper;
        private readonly UserMapper _userMapper;

        public RolesRepository(ProjectManagementDbContext context, RoleMapper roleMapper, UserMapper userMapper)
        {
            _context = context;
            _roleMapper = roleMapper;
            _userMapper = userMapper;
        }

        public async Task<Role> GetRoleById(int id)
        {
            var roleEntity = await _context.Roles.FindAsync(id);
            return await _roleMapper.MapEntityToRole(roleEntity);
        }

        public async Task<List<Role>> GetAll()
        {
            var roleEntity = await _context.Roles
                .AsNoTracking()
                .ToListAsync();

            List<Role> roles = new List<Role>();
            foreach (var role in roleEntity)
            {
                roles.Add(await _roleMapper.MapEntityToRole(role));
            }

            return roles;
        }

        public async Task<int> Create(Role role, int createdByUserId)
        {
            var admin = await _context.Users.FindAsync(createdByUserId);
            var creterByUserId = _userMapper.MapEntityToUser(admin);

            if(admin.Role.Name != "admin") 
            {
                throw new Exception("Only admins can create roles");
            }

            var roleEntity = _roleMapper.MapRoleToEntity(role);

            await _context.AddAsync(roleEntity);
            await _context.SaveChangesAsync();

            return roleEntity.Id;
        }
    }
}

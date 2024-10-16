using Microsoft.EntityFrameworkCore;
using Project_Job.DataAccess;
using ProjectManagement.Core.Models;
using ProjectManagement.Architecture.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagement.Architecture.Mappers;
using ProjectManagement.Core.Abstactions;
using Microsoft.VisualBasic;

namespace ProjectManagement.Architecture.UserRepository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ProjectManagementDbContext _context;
        private readonly RolesRepository _rolesRepository;
        private readonly UserMapper _userMapper;
        private readonly RoleMapper _roleMapper;

        public UsersRepository(ProjectManagementDbContext context, UserMapper userMapper, RoleMapper roleMapper)
        {
            _context = context;
            _userMapper = userMapper;
            _roleMapper = roleMapper;
        }

        public async Task<List<User>> GetAll()
        {
            var UserEntity = await _context.Users
                .Include(u => u.Role)
                .AsNoTracking()
                .ToListAsync();

            List<User> users = new List<User>();
            foreach (var user in UserEntity) 
            {
                users.Add(await _userMapper.MapEntityToUser(user));
            }

            return users;
        }

        public async Task<User> GetUserById(int id)
        {
            UserEntity userEntity = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Id == id);
            return await _userMapper.MapEntityToUser(userEntity);
        }


        public async Task<User> GetUserByLogin(string? login) 
        {
            UserEntity userEntity = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.login == login);

            if(userEntity == null) 
            {
                return null;
            }
            return await _userMapper.MapEntityToUser(userEntity);
        }

        public async Task<int> Create(User user, int createdByUserId)
        {
            var createdByUser = await GetUserById(createdByUserId);
            if(createdByUser.roleName != "admin") 
            {
                throw new UnauthorizedAccessException("Only admins users can create new users");
            }
            var role = _context.Roles.FirstOrDefault(u => u.Name == user.roleName);
            if(role == null) 
            {
                Role newRole = new Role { Name = user.roleName };
                RoleEntity roleEntity = _roleMapper.MapRoleToEntity(newRole);
                await _context.Roles.AddAsync(roleEntity);
                await _context.SaveChangesAsync();
                role = roleEntity;
            }
            var checkUser = _context.Users.FirstOrDefault(u => u.login == user.login);
            if(checkUser != null) 
            {
                return 0;
            }
            var userEntity = _userMapper.MapUserToEntity(user, role);
            await _context.Users.AddAsync(userEntity);
            await _context.SaveChangesAsync();

            return userEntity.Id;
        }

        public async Task<int> Update(User user, int createdByUserId)
        {
            var createdByUser = await GetUserById(createdByUserId);
            if (createdByUser.roleName != "admin")
            {
                throw new UnauthorizedAccessException("Only admins users can update users");
            }
            var userEntity = 
            await _context.Users
                .Where(u => u.Id == user.Id)
                .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.login, u => user.login)
                .SetProperty(u => u.Role, u => _context.Roles.Find(u.Role.Id))
                );
            return user.Id;
        }

        public async Task<int> Delete(int id)
        {
            await _context.Users
                .Where(u => u.Id == id)
                .ExecuteDeleteAsync();

            return id;
        }
    }
}

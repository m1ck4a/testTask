using ProjectManagement.Architecture.Entities;
using ProjectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Architecture.Mappers
{
    public class UserMapper
    {
        public Task<User> MapEntityToUser (UserEntity userEntity)
        {
            return User.Create(
                userEntity.Id,
                userEntity.login,
                userEntity.password,
                userEntity.Role?.Name
                );
        }

        public UserEntity MapUserToEntity(User user, RoleEntity role)
        {
            return new UserEntity
            {
                login = user.login,
                password = user.password,
                Role = role
            };
        }
    }

}

using ProjectManagement.Architecture.Entities;
using ProjectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Architecture.Mappers
{
    public class RoleMapper
    {
        public Task<Role> MapEntityToRole(RoleEntity roleEntity)
        {
            return Role.Create(roleEntity.Name);
        }

        public RoleEntity MapRoleToEntity(Role role)
        {
            return new RoleEntity
            {
                Name=role.Name
            };
        }
    }
}

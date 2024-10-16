using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ProjectManagement.Architecture.Entities
{
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? login { get; set; }

        public string? password { get; set; }
        public int ProjectsId { get; set; }

        [Required]
        public RoleEntity? Role { get; set; }
    }   
}
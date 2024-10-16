using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProjectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Architecture.Entities
{
    public class ProjectEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Status {  get; set; }
        [ForeignKey("UsersId")]
        public int UsersId { get; set; }
    }
}

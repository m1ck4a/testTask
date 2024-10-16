using ProjectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Core.Abstractions
{
    public interface IProjectsRepository
    {
        Task <List<Project>> GetAllProjects(User user);
        Task<List<Project>> GetAllProjects();
        Task<Project> GetProjectById(int id);
        Task<int> CreateProject(Project project, int CreatedByUserId);
        Task<int> UpdateProject(Project project);
        Task<bool> UpdateProjectStatus(string status,int id);
    }
}

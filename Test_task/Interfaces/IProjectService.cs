using ProjectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_task.Interfaces
{
    public interface IProjectService
    {
        Task CreateProjectAsync(Project project, int createdByUserId);
        Task<int> UpdateProjectAsync(Project project);
        Task<Project> GetProjectByIdAsync(int id);
        Task<List<Project>> GetProjectsAsync(User user);
        Task<List<Project>> GetAllProjectsAsync();
        Task<bool> UpdateProjectStatus(string status, int id);
    }

}

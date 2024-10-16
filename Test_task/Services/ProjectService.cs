using ProjectManagement.Core.Models;
using ProjectManagement.Core.Abstactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test_task.Interfaces;
using ProjectManagement.Core.Abstractions;

namespace Test_task.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectsRepository _projectsRepository;

        public ProjectService(IProjectsRepository projectRepository)
        {
            _projectsRepository = projectRepository;
        }

        public async Task CreateProjectAsync(Project project, int createdByUserId)
        {
            await _projectsRepository.CreateProject(project, createdByUserId);
        }

        public async Task<int> UpdateProjectAsync(Project project)
        {
            return await _projectsRepository.UpdateProject(project);
        }

        public async Task<Project> GetProjectByIdAsync(int id)
        {
            return await _projectsRepository.GetProjectById(id);
        }

        public async Task<List<Project>> GetProjectsAsync(User user)
        {
            return await _projectsRepository.GetAllProjects(user);
        }

        public async Task<List<Project>> GetAllProjectsAsync()
        {
            return await _projectsRepository.GetAllProjects();
        }

        public async Task<bool> UpdateProjectStatus(string status, int id) 
        {
            return await _projectsRepository.UpdateProjectStatus(status, id);
        }

    }

}

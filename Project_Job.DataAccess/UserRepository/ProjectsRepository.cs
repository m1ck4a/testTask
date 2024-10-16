using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
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
    public class ProjectsRepository : IProjectsRepository
    {
        private readonly ProjectManagementDbContext _context;
        private readonly ProjectMapper _projectMapper;

        public ProjectsRepository(ProjectManagementDbContext context, ProjectMapper mapper) 
        {
            _context = context;
            _projectMapper = mapper;
        }

        public async Task<int> CreateProject(Project project, int CreatedByUserId)
        {
            var createUser = await _context.Users.FindAsync(CreatedByUserId);

            if(createUser.Role.Name != "admin") 
            {
                throw new UnauthorizedAccessException("only admins can create new project");
            }

            var projectEntity = _projectMapper.MapProjectToEntity(project);
            await _context.Projects.AddAsync(projectEntity);
            await _context.SaveChangesAsync();

            return projectEntity.Id;
        }

        public async Task<List<Project>> GetAllProjects(User user)
        {
            var ProjectEntity = await _context.Projects
                .Where(u => u.UsersId == user.Id)
                .AsNoTracking()
                .ToListAsync();

            return _projectMapper.MapEntitiesToProjects(ProjectEntity);
        }


        public async Task<List<Project>> GetAllProjects()
        {
            var ProjectEntity = await _context.Projects
                .AsNoTracking()
                .ToListAsync();

            return _projectMapper.MapEntitiesToProjects(ProjectEntity);
        }

        public async Task<Project> GetProjectById(int id)
        {
            var projectEntity = await _context.Projects.FindAsync(id);
            if(projectEntity == null) 
            {
                return null;
            }

            return _projectMapper.MapEntityToProject(projectEntity);
        }

        public async Task<int> UpdateProject(Project project)
        {
            var projectEntity = await _context.Projects.FindAsync(project.Id);
            if(projectEntity == null) 
            {
                return -1;
            }
            projectEntity.UsersId = project.UserId;
            await _context.SaveChangesAsync();
            var userEntity =  _context.Users.Where(u => u.Id == projectEntity.UsersId).FirstOrDefault();
            userEntity.ProjectsId = project.Id; await _context.SaveChangesAsync();

            return project.Id;
        }

        public async Task<bool> UpdateProjectStatus(string status, int id)
        {
            if (!string.IsNullOrWhiteSpace(status))
            {
                await _context.Projects.Where(p => p.Id == id)
                    .ExecuteUpdateAsync(s => s
                    .SetProperty(p => p.Status, p => status));
                return true;
            }
            return false;
        }
    }
}

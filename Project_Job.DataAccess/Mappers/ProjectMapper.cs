using ProjectManagement.Architecture.Entities;
using ProjectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagement.Architecture.Mappers
{
    public class ProjectMapper
    {
        public Project MapEntityToProject(ProjectEntity projectEntity)
        {
            Project project;
            if (projectEntity.UsersId != 0)
            {
                project = new Project
                {
                    Id = projectEntity.Id,
                    Name = projectEntity.Name,
                    UserId = projectEntity.UsersId,
                    Status = projectEntity.Status
                };
            }
            else 
            {
                project = new Project
                {
                    Id = projectEntity.Id,
                    Name = projectEntity.Name
                };
            }
            return project;
        }

        public Project MapEntityToProjectWithStatus(ProjectEntity projectEntity)
        {
            Project project = new Project
            {
                Id = projectEntity.Id,
                Name = projectEntity.Name,
                Status = projectEntity.Status
            };
            return project;
        }

        public List<Project> MapEntitiesToProjects(List<ProjectEntity> projectEntities)
        {
            return projectEntities.Select(MapEntityToProject).ToList();
        }

        public ProjectEntity MapProjectToEntity(Project project)
        {
            if (project.UserId != 0)
            {
                return new ProjectEntity
                {
                    Name = project.Name,
                    UsersId = project.UserId
                };
            }
            else
            {
                return new ProjectEntity
                {
                    Name = project.Name
                };
            }
        }

        public ProjectEntity MapProjectToEntityWithStatus(Project project)
        {
            return new ProjectEntity
            {
                Name = project.Name,
                Status = project.Status
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Milestones.Models;
using Milestones_Backend.Models;

namespace Milestones.Services
{
    /// <summary>
    /// Defines the rulls, that which methods must be includes in <see cref="ProjectService"/> class.
    /// </summary>
    public interface IProjectService
    {
        public Task<List<Project>> GetAll();
        public Task<List<Project>> GetByName(string name);
        public Task<List<Project>> GetByStartAfter(DateTime datetime);
        public Task<List<Project>> GetByEndBefore(DateTime datetime);
        public Task<Project> GetById(string id);
        public Task DeleteProject(string id);
        public Task CreateProject(Project newProject);
        public Task UpdateProject(string id, Project updatedProject);
    }
}
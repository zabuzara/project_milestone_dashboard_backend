using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Milestones.Models;
using Milestones_Backend.Models;

namespace Milestones.Services
{
    /// <summary>
    /// Defines the rulls, that which methods must be includes in <see cref="MilestoneService"/> class.
    /// </summary>
    public interface IMilestoneService
    {
        public Task<List<Milestone>> GetAll();
        public Task<Milestone> GetById(string id);
        public Task<List<Milestone>> GetByName(string name);
        public Task<List<Milestone>> GetByDescription(string description);
        public Task<List<Milestone>> GetByProjectId(string id);
        public Task<List<Milestone>> GetByMemberId(string id);
        public Task<List<Milestone>> GetByMemberName(string name);
        public Task<List<Milestone>> GetByStatus(Status status);
        public Task<List<Milestone>> GetByStartAfter(DateTime datetime);
        public Task<List<Milestone>> GetByEndBefore(DateTime datetime);
        public Task DeleteMilestone(string id);
        public Task CreateMilestone(Milestone newMilestone);
        public Task UpdateMilestone(string id, Milestone updatedMilestone);
    }
}
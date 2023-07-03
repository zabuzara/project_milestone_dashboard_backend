using System.Collections.Generic;
using System.Threading.Tasks;
using Milestones.Models;

namespace Milestones.Services
{
    /// <summary>
    /// Defines the rulls, that which methods must be includes in <see cref="MemberService"/> class.
    /// </summary>
    public interface IMemberService
    {
        public Task<List<Member>> GetAll();
        public Task<Member> GetById(string id);
        public Task<List<Member>> GetByName(string name);
        public Task DeleteMember(string id);
        public Task CreateMember(Member newMember);
        public Task UpdateMember(string id, Member updatedMember);
    }
}
using Milestones.Models;
using Milestones.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using MongoDB.Driver;

namespace Milestones.Controllers
{
    /// <summary>
    /// Member controller class for using with services.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly MemberService memberService;

        /// <summary>
        /// Initializes a new instance  
        /// for using by HTTP operations
        /// </summary>
        /// <param name="memberService"></param>
        public MemberController(MemberService memberService)
        {
            this.memberService = memberService;
        }


        /// <summary>
        /// Returns the List of members
        /// with HTTP-GET
        /// </summary>
        /// <returns>the List of members</returns>
        [HttpGet(nameof(GetAll))]
        public async Task<List<Member>> GetAll()
        {
            return await this.memberService.GetAll();
        }


        /// <summary>
        /// Returns the member of given memberId
        /// with HTTP-GET operation
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the member of given memberId</returns>
        [HttpGet(nameof(GetById) + "/{id:length(24)}")]
        public async Task<ActionResult<Member>> GetById(string id)
        {
            try
            {
                Member member = await this.memberService.GetById(id);
                if (member is null)
                    return CreatedAtAction(nameof(GetById), new { success = false, message = "Member not exists" });

                return Ok(member);
            } 
            catch (FormatException e)
            {
                return CreatedAtAction(nameof(GetById), new { success = false, message = e.Message });
            }
        }


        /// <summary>
        /// Returns the members of given name
        /// with HTTP-GET operation
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the members of given name</returns>
        [HttpGet(nameof(GetByName) + "/{name:length(1,255)}")]
        public async Task<List<Member>> GetByName(string name)
        {
            return await this.memberService.GetByName(name);
        }


        /// <summary>
        /// Writes a new member in the MongoDB database
        /// with HTTP-POST operation
        /// </summary>
        /// <param name="newMember#"></param>
        /// <returns>the created <see cref="CreatedAtActionResult"/> of the response</returns>
        [HttpPost(nameof(CreateMember))]
        public async Task<ActionResult<Member>> CreateMember(Member newMember)
        {
            if (newMember is null)
                return CreatedAtAction(nameof(CreateMember), new { success = false, message = "Member is null" });

            if (Member.IsMissingProperties(newMember))
                return CreatedAtAction(nameof(CreateMember), new { success = false, message = "Member property is missing" });

            Task<List<Member>> members = this.memberService.GetAll();
            if (!members.Result.Exists(member => member == newMember))
            {
                await this.memberService.CreateMember(newMember);
                return CreatedAtAction(nameof(CreateMember), new { id = newMember.Id }, newMember);
            }
            else {
                return CreatedAtAction(nameof(CreateMember), new { success = false, message = "Member-Duplicate not allowed" });
            }
        }


        /// <summary>
        /// Updates the old member from given id
        /// with the given updatedMember instance in the MongoDB database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedMember"></param>
        /// <returns>the created <see cref="CreatedAtActionResult"/> of the response</returns>
        [HttpPut(nameof(UpdateMember) + "/{id:length(24)}")]
        public async Task<ActionResult<Member>> UpdateMember(string id, Member updatedMember)
        {
            try
            {
                Member member = await this.memberService.GetById(id);
                if (member is null)
                    return CreatedAtAction(nameof(UpdateMember), new { success = false, message = "Member not exists" });

                if (Member.IsMissingProperties(updatedMember))
                    return CreatedAtAction(nameof(UpdateMember), new { success = false, message = "Member property is missing" });

                Task<List<Member>> members = this.memberService.GetAll();
                if (members.Result.Exists(member => member == updatedMember && !member.Id.Equals(updatedMember.Id)))
                {
                    return CreatedAtAction(nameof(UpdateMember), new { success = false, message = "Member-Duplicate not allowed" });
                }

                await this.memberService.UpdateMember(id, updatedMember);
                return CreatedAtAction(nameof(UpdateMember), new { success = true, message = "Member updated" });
            }
            catch (Exception e)
            {
                return CreatedAtAction(nameof(UpdateMember), new { success = false, message = e.Message });
            }
        }


        /// <summary>
        /// Deletes the member of given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the created <see cref="CreatedAtActionResult"/> of the response</returns>
        [HttpDelete(nameof(DeleteMember) + "/{id:length(24)}")]
        public async Task<ActionResult<Member>> DeleteMember(string id)
        {
            try
            {
                Member member = await this.memberService.GetById(id);
                if (member is null)
                    return CreatedAtAction(nameof(DeleteMember), new { success = false, message = "Member not exists" });
                await this.memberService.DeleteMember(id);
                return CreatedAtAction(nameof(DeleteMember), new { success = true, message = "Member removed" });
            }
            catch (Exception e)
            {
                return CreatedAtAction(nameof(DeleteMember), new { success = false, message = e.Message });
            }
        }
    }
}
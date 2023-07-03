using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Milestones.Services;
using Milestones.Models;
using System;
using Milestones_Backend.Models;

namespace Milestones.Controllers
{
    /// <summary>
    /// Milestone controller class for using with services.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MilestoneController : ControllerBase
    {
        private readonly MilestoneService milestoneService;
        private readonly MemberService memberService;
        private readonly ProjectService projectService;


        /// <summary>
        /// Initializes a new instance and 
        /// initializes the
        /// <see cref="ProjectService"/>,
        /// <see cref="MilestoneService"/>,
        /// <see cref="MemberService"/> 
        /// for using by HTTP operations
        /// </summary>
        /// <param name="milestoneService"></param>
        /// <param name="memberService"></param>
        /// <param name="projectService"></param>
        public MilestoneController(MilestoneService milestoneService, MemberService memberService, ProjectService projectService)
        {
            this.milestoneService = milestoneService;
            this.memberService = memberService;
            this.projectService = projectService;
        }


        /// <summary>
        /// Returns the List of milestones
        /// with HTTP-GET
        /// </summary>
        /// <returns>the List of milestones</returns>
        [HttpGet(nameof(GetAll))]
        public async Task<List<Milestone>> GetAll()
        {
            return await this.milestoneService.GetAll();
        }


        /// <summary>
        /// Returns the milestone of given id
        /// with HTTP-GET operation
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the milestone of given id</returns>
        [HttpGet(nameof(GetById) + "/{id:length(24)}")]
        public async Task<ActionResult<Milestone>> GetById(string id)
        {
            try
            {
                Milestone milestone = await this.milestoneService.GetById(id);
                if (milestone is null)
                    return CreatedAtAction(nameof(GetById), new { success = false, message = "Milestone not exists" });

                return Ok(milestone);
            }
            catch (Exception e)
            {
                return CreatedAtAction(nameof(GetById), new { success = false, message = e.Message });
            }
        }


        /// <summary>
        /// Returns the List of milestones of given name
        /// with HTTP-GET
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the List of milestones</returns>
        [HttpGet(nameof(GetByName) + "/{name:length(1,255)}")]
        public async Task<List<Milestone>> GetByName(string name)
        {
            return await this.milestoneService.GetByName(name);
        }


        /// <summary>
        /// Returns the List of milestones of given description
        /// with HTTP-GET
        /// </summary>
        /// <param name="description"></param>
        /// <returns>the List of milestones</returns>
        [HttpGet(nameof(GetByDescription) + "/{description:length(1,1000)}")]
        public async Task<List<Milestone>> GetByDescription(string description)
        {
            return await this.milestoneService.GetByDescription(description);
        }


        /// <summary>
        /// Returns the List of milestones of given member id
        /// with HTTP-GET
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the List of milestones</returns>
        [HttpGet(nameof(GetByMemberId) + "/{id:length(24)}")]
        public async Task<ActionResult<List<Milestone>>> GetByMemberId(string id)
        {
            try
            {
                return await this.milestoneService.GetByMemberId(id);
            }
            catch (Exception e)
            {
                return CreatedAtAction(nameof(GetByMemberId), new { success = false, message = e.Message });
            }
        }


        /// <summary>
        /// Returns the List of milestones of given project id
        /// with HTTP-GET
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the List of milestones</returns>
        [HttpGet(nameof(GetByProjectId) + "/{id:length(24)}")]
        public async Task<ActionResult<List<Milestone>>> GetByProjectId(string id)
        {
            try
            {
                return await this.milestoneService.GetByProjectId(id);
            }
            catch (Exception e)
            {
                return CreatedAtAction(nameof(GetByProjectId), new { success = false, message = e.Message });
            }
        }


        /// <summary>
        /// Returns the List of milestones of given member firstname or lastname
        /// with HTTP-GET
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the List of milestones</returns>
        [HttpGet(nameof(GetByMemberName) + "/{name:length(1,255)}")]
        public async Task<ActionResult<List<Milestone>>> GetByMemberName(string name)
        {
            try
            {
                return await this.milestoneService.GetByMemberName(name);
            }
            catch (Exception e)
            {
                return CreatedAtAction(nameof(GetByMemberName), new { success = false, message = e.Message });
            }
        }


        /// <summary>
        /// Returns the List of milestones by status
        /// with HTTP-GET
        /// </summary>
        /// <returns>the List of milestones</returns>
        [HttpGet(nameof(GetByStatus) + "/{status}")]
        public async Task<List<Milestone>> GetByStatus(Status status)
        {
            return await this.milestoneService.GetByStatus(status);
        }


        /// <summary>
        /// Returns the List of milestones, they starts after given datetime
        /// with HTTP-GET
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        [HttpGet(nameof(GetByStartAfter) + "/{datetime}")]
        public async Task<List<Milestone>> GetByStartAfter(DateTime datetime)
        {
            return await this.milestoneService.GetByStartAfter(datetime);
        }


        /// <summary>
        /// Returns the List of milestones, they ends before given datetime
        /// with HTTP-GET
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        [HttpGet(nameof(GetByEndBefore) + "/{datetime}")]
        public async Task<List<Milestone>> GetByEndBefore(DateTime datetime)
        {
            return await this.milestoneService.GetByEndBefore(datetime);
        }


        /// <summary>
        /// Writes a new milestone in the MongoDB database
        /// with HTTP-POST operation
        /// </summary>
        /// <param name="newMilestone"></param>
        /// <returns>the created <see cref="CreatedAtActionResult"/> of the response</returns>
        [HttpPost(nameof(CreateMilestone))]
        public async Task<ActionResult<Milestone>> CreateMilestone(Milestone newMilestone)
        {
            List<Milestone> milestones = this.milestoneService.GetAll().Result;
            List<Project> projects = this.projectService.GetAll().Result;

            if (Milestone.IsMissingProperties(newMilestone))
                return CreatedAtAction(nameof(CreateMilestone), new { success = false, message = "Milestone property is missing" });

            if (!projects.Exists(project => project.Id.Equals(newMilestone.ProjectReference)))
                 return CreatedAtAction(nameof(CreateMilestone), new { success = false, message = "Project not exists" });
          
            if (!milestones.Exists(milestone => milestone == newMilestone))
            {
                List<Member> members = this.memberService.GetAll().Result;

                bool memebrExists = true;
                for (int i = 0; i < newMilestone.Members.Count; i++)
                {
                    if (!members.Exists(member => newMilestone.Members[i].Id == member.Id && newMilestone.Members[i] == member))
                    {
                        memebrExists = false;
                        break;
                    }
                }

                if (!memebrExists)
                    return CreatedAtAction(nameof(CreateMilestone), new { success = false, message = "Member/s not exists" });

                if (newMilestone.Start > newMilestone.End)
                    return CreatedAtAction(nameof(CreateMilestone), new { success = false, message = "Invalid Start Date" });

                await this.milestoneService.CreateMilestone(newMilestone);
                return CreatedAtAction(nameof(CreateMilestone), new { id = newMilestone.Id }, newMilestone);
            }
            else
            {
                return CreatedAtAction(nameof(CreateMilestone), new { success = false, message = "Milestone-Duplicate not allowed" });
            }
        }


        /// <summary>
        /// Updates the old milestone from given id
        /// with the given updatedMilestone instance in the MongoDB database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedMilestone"></param>
        /// <returns>the created <see cref="CreatedAtActionResult"/> of the response</returns>
        [HttpPut(nameof(UpdateMilestone) + "/{id:length(24)}")]
        public async Task<ActionResult<Milestone>> UpdateMilestone(string id, Milestone updatedMilestone)
        {
            try
            {
                Milestone milestone = await this.milestoneService.GetById(id);
                if (milestone is null)
                    return CreatedAtAction(nameof(UpdateMilestone), new { success = false, message = "Milestone not exists" });

                List<Project> projects = this.projectService.GetAll().Result;
                if (!projects.Exists(project => project.Id.Equals(updatedMilestone.ProjectReference)))
                    return CreatedAtAction(nameof(UpdateMilestone), new { success = false, message = "Project not exists" });

                List<Member> members = this.memberService.GetAll().Result;
                bool memebrExists = true;
                for (int i = 0; i < updatedMilestone.Members.Count; i++)
                {
                    if (!members.Exists(member => updatedMilestone.Members[i].Id == member.Id && updatedMilestone.Members[i] == member))
                    {
                        memebrExists = false;
                        break;
                    }
                }
                if (!memebrExists)
                    return CreatedAtAction(nameof(CreateMilestone), new { success = false, message = "Member/s not exists" });

                if (Milestone.IsMissingProperties(updatedMilestone))
                    return CreatedAtAction(nameof(UpdateMilestone), new { success = false, message = "Milestone property is missing" });

                List<Milestone> milestones = this.milestoneService.GetAll().Result;
                if (milestones.Exists(milestone => milestone == updatedMilestone && milestone.Id != updatedMilestone.Id))
                {
                    return CreatedAtAction(nameof(UpdateMilestone), new { success = false, message = "Milestone-Duplicate not allowed" });
                }

                if (updatedMilestone.Start > updatedMilestone.End)
                    return CreatedAtAction(nameof(CreateMilestone), new { success = false, message = "Invalid Start Date" });

                await this.milestoneService.UpdateMilestone(id, updatedMilestone);
                return CreatedAtAction(nameof(UpdateMilestone), new { success = true, message = "Milestone updated" });
            }
            catch (Exception e)
            {
                return CreatedAtAction(nameof(UpdateMilestone), new { success = false, message = e.Message });
            }
        }


        /// <summary>
        /// Deletes the milestone of given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the created <see cref="CreatedAtActionResult"/> of the response</returns>
        [HttpDelete(nameof(DeleteMilestone) + "/{id:length(24)}")]
        public async Task<ActionResult<Milestone>> DeleteMilestone(string id)
        {
            try { 
                Milestone milestone = await this.milestoneService.GetById(id);
                if (milestone is null)
                    return CreatedAtAction(nameof(DeleteMilestone), new { success = false, message = "Milestone not exists" });
                await this.milestoneService.DeleteMilestone(id);
                return CreatedAtAction(nameof(DeleteMilestone), new { success = true, message = "Milestone removed" });
            }
            catch (Exception e)
            {
                return CreatedAtAction(nameof(DeleteMilestone), new { success = false, message = e.Message});
            }
        }
    }
}
using Milestones.Models;
using Milestones.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Milestones_Backend.Models;

namespace Milestones.Controllers
{
    /// <summary>
    /// Project controller class for using with services.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectService projectService;
        private readonly MilestoneService milestoneService;


        /// <summary>
        /// Initializes a new instance and 
        /// initializes the <see cref="ProjectService"/> 
        /// for using by HTTP operations
        /// </summary>
        /// <param name="projectService"></param>
        /// <param name="milestoneService"></param>
        public ProjectController(ProjectService projectService, MilestoneService milestoneService)
        {
            this.projectService = projectService;
            this.milestoneService = milestoneService;
        }


        /// <summary>
        /// Returns the List of projects
        /// with HTTP-GET
        /// </summary>
        /// <returns>the List of projects</returns>
        [HttpGet(nameof(GetAll))]
        public Task<List<Project>> GetAll()
        {
            List<Project> projects = this.projectService.GetAll().Result;
            List<Milestone> milestones = this.milestoneService.GetAll().Result;

            for (int projectIndex = 0; projectIndex < projects.Count; projectIndex++)
            {
                projects[projectIndex].Milestones.Clear();

                for (int milestoneIndex = 0; milestoneIndex < milestones.Count; milestoneIndex++)
                {
                    if (projects[projectIndex].Id.Equals(milestones[milestoneIndex].ProjectReference))
                        projects[projectIndex].Milestones.Add(milestones[milestoneIndex]);
                }
            }
            return Task.FromResult(projects);
        }


        /// <summary>
        /// Returns the project of given id
        /// with HTTP-GET operation
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the project of given id</returns>
        [HttpGet(nameof(GetById) + "/{id:length(24)}")]
        public async Task<ActionResult<Project>> GetById(string id)
        {
            try
            {
                Project project = await this.projectService.GetById(id);
                if (project is null)
                    return CreatedAtAction(nameof(GetById), new { success = false, message = "Project not exists" });

                project.Milestones.Clear();
                List<Milestone> milestones = this.milestoneService.GetByProjectId(project.Id).Result;
                project.Milestones.AddRange(milestones);

                return Ok(project);
            }
            catch (FormatException e)
            {
                return CreatedAtAction(nameof(GetById), new { success = false, message = e.Message });
            }
        }


        /// <summary>
        /// Returns the List of projects of given name
        /// with HTTP-GET
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the List of projects</returns>
        [HttpGet(nameof(GetByName) + "/{name:length(1,255)}")]
        public Task<List<Project>> GetByName(string name)
        {
            List<Project> projects = this.projectService.GetByName(name).Result;
            foreach (Project project in projects)
            {
                project.Milestones.Clear();
                List<Milestone> milestones = this.milestoneService.GetByProjectId(project.Id).Result;
                project.Milestones.AddRange(milestones);
            }
            return Task.FromResult(projects);
        }


        /// <summary>
        /// Returns the List of projects of given member id
        /// with HTTP-GET
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the List of projects</returns>
        [HttpGet(nameof(GetByMemberId) + "/{id:length(24)}")]
        public async Task<ActionResult<List<Project>>> GetByMemberId(string id)
        {
            try
            {
                List<Project> projects = this.projectService.GetAll().Result;
                foreach(Project project in projects)
                {
                    bool memberFound = false;
                    List<Milestone> milestones = this.milestoneService.GetByProjectId(project.Id).Result;
                    foreach (Milestone milestone in milestones)
                        foreach(Member member in milestone.Members)
                            if (member.Id == id)
                                memberFound = true;

                    if (memberFound)
                        project.Milestones = milestones;
                    else
                        project.Milestones = new List<Milestone>();
                }
                projects.RemoveAll(project => project.Milestones.Count == 0);
                return await Task.FromResult(projects);
            }
            catch (FormatException e)
            {
                return CreatedAtAction(nameof(GetByMemberId), new { success = false, message = e.Message });
            }
        }


        /// <summary>
        /// Returns the List of projects of given member name
        /// with HTTP-GET
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the List of projects</returns>
        [HttpGet(nameof(GetByMemberName) + "/{name:length(1,255)}")]
        public async Task<ActionResult<List<Project>>> GetByMemberName(string name)
        {
            try
            {
                List<Project> projects = this.projectService.GetAll().Result;
                foreach (Project project in projects)
                {
                    bool memberFound = false;
                    List<Milestone> milestones = this.milestoneService.GetByProjectId(project.Id).Result;
                    foreach (Milestone milestone in milestones)
                        foreach (Member member in milestone.Members)
                            if (member.Firstname.ToLower().Contains(name.ToLower()) || member.Lastname.ToLower().Contains(name.ToLower()))
                                memberFound = true;

                    if (memberFound)
                        project.Milestones = milestones;
                    else
                        project.Milestones = new List<Milestone>();
                }
                projects.RemoveAll(project => project.Milestones.Count == 0);
                return await Task.FromResult(projects);
            }
            catch (FormatException e)
            {
                return CreatedAtAction(nameof(GetByMemberName), new { success = false, message = e.Message });
            }
        }


        /// <summary>
        /// Returns the List of projects of given status
        /// with HTTP-GET
        /// </summary>
        /// <param name="status"></param>
        /// <returns>the List of projects</returns>
        [HttpGet(nameof(GetByStatus) + "/{status}")]
        public async Task<ActionResult<List<Project>>> GetByStatus(Status status)
        {
            try
            {
                List<Project> projects = this.projectService.GetAll().Result;
                foreach (Project project in projects)
                {
                    project.Milestones = this.milestoneService.GetByProjectId(project.Id).Result;
                }
                switch (status)
                {
                    case Status.COMPLETED:
                        projects.RemoveAll(project => project.Milestones.Count == 0 || !project.Milestones.TrueForAll(milestone => (bool)milestone.IsCompleted));
                        break;
                    case Status.OPENS:
                        projects.RemoveAll(project => project.End <= DateTime.UtcNow);
                        projects.RemoveAll(project => project.Milestones.TrueForAll(milestone => (bool)milestone.IsCompleted));
                        break;
                    case Status.EXPIRED:
                        projects.RemoveAll(project => project.End > DateTime.UtcNow);
                        projects.RemoveAll(project => project.Milestones.TrueForAll(milestone => (bool)milestone.IsCompleted));
                        break;
                    default: break;
                }

                return await Task.FromResult(projects);
            }
            catch (FormatException e)
            {
                return CreatedAtAction(nameof(GetByStatus), new { success = false, message = e.Message });
            }
        }


        /// <summary>
        /// Returns the List of projects of given milestone name
        /// with HTTP-GET
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the List of projects</returns>
        [HttpGet(nameof(GetByMilestoneName) + "/{name:length(1,255)}")]
        public async Task<List<Project>> GetByMilestoneName(string name)
        {
            List<Project> projects = this.projectService.GetAll().Result;
            foreach (Project project in projects)
            {
                List<Milestone> milestones = this.milestoneService.GetByProjectId(project.Id).Result;
                if (milestones.Exists(milestone => milestone.Name.ToLower().Contains(name.ToLower())))
                    project.Milestones = milestones;
            }
            projects.RemoveAll(projects => projects.Milestones.Count == 0);
            return await Task.FromResult(projects);
        }


        /// <summary>
        /// Returns the List of projects of given milestone description
        /// with HTTP-GET
        /// </summary>
        /// <param name="description"></param>
        /// <returns>the List of projects</returns>
        [HttpGet(nameof(GetByMilestoneDescription) + "/{description:length(1,1000)}")]
        public async Task<List<Project>> GetByMilestoneDescription(string description)
        {
            List<Project> projects = this.projectService.GetAll().Result;
            foreach (Project project in projects)
            {
                List<Milestone> milestones = this.milestoneService.GetByProjectId(project.Id).Result;
                if (milestones.Exists(milestone => milestone.Description.ToLower().Contains(description.ToLower())))
                    project.Milestones = milestones;
            }
            projects.RemoveAll(projects => projects.Milestones.Count == 0);
            return await Task.FromResult(projects);
        }


        /// <summary>
        /// Returns the List of projects, they starts after given datetime
        /// with HTTP-GET
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns>the List of projects</returns>
        [HttpGet(nameof(GetByStartAfter) + "/{datetime}")]
        public async Task<List<Project>> GetByStartAfter(DateTime datetime)
        {
            return await this.projectService.GetByStartAfter(datetime);
        }


        /// <summary>
        /// Returns the List of projects, they ends before given datetime
        /// with HTTP-GET
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns>the List of projects</returns>
        [HttpGet(nameof(GetByEndBefore) + "/{datetime}")]
        public async Task<List<Project>> GetByEndBefore(DateTime datetime)
        {
            return await this.projectService.GetByEndBefore(datetime);
        }


        /// <summary>
        /// Writes a new project in the MongoDB database
        /// with HTTP-POST operation
        /// </summary>
        /// <param name="newProject"></param>
        /// <returns>the created <see cref="CreatedAtActionResult"/> of the response</returns>
        [HttpPost(nameof(CreateProject))]
        public async Task<ActionResult<Project>> CreateProject(Project newProject)
        {
            if (Project.IsMissingProperties(newProject))
                return CreatedAtAction(nameof(CreateProject), new { success = false, message = "Project property is missing" });

            List<Project> projects = this.projectService.GetAll().Result;
            for (int projectIndex = 0; projectIndex < projects.Count; projectIndex++)
            {
                if (projects[projectIndex] == newProject)
                    return CreatedAtAction(nameof(CreateProject), new { success = false, message = "Project-Duplicate not allowed" });
            }
            newProject.Milestones.Clear();
            await this.projectService.CreateProject(newProject);
            return CreatedAtAction(nameof(CreateProject), new { id = newProject.Id }, newProject);  
        }


        /// <summary>
        /// Updates the old project from given projectId
        /// with the given updatedProject instance in the MongoDB database
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedProject"></param>
        /// <returns>the created <see cref="CreatedAtActionResult"/> of the response</returns>
        [HttpPut(nameof(UpdateProject) + "/{id:length(24)}")]
        public async Task<ActionResult<Project>> UpdateProject(string id, Project updatedProject)
        {
            try {
                Project project = await this.projectService.GetById(id);
                if (project is null)
                    return CreatedAtAction(nameof(UpdateProject), new { success = false, message = "Project not exists" });

                if (Project.IsMissingProperties(updatedProject))
                    return CreatedAtAction(nameof(UpdateProject), new { success = false, message = "Project property is missing" });

                project.Milestones.Clear();
                List<Milestone> milestones = this.milestoneService.GetByProjectId(id).Result;
                for (int milestoneIndex = 0; milestoneIndex < updatedProject.Milestones.Count; milestoneIndex++)
                {
                    if (string.IsNullOrEmpty(updatedProject.Milestones[milestoneIndex].Id) || 
                        (string.IsNullOrEmpty(updatedProject.Milestones[milestoneIndex].Id) && milestones.Count > 1 && milestones.Exists(milestone => milestone != updatedProject.Milestones[milestoneIndex])))
                    {
                        await this.milestoneService.CreateMilestone(updatedProject.Milestones[milestoneIndex]);
                    }
                }

                await this.projectService.UpdateProject(id, updatedProject);
                return CreatedAtAction(nameof(UpdateProject), new { success = true, message = "Project updated" });
            }
            catch (Exception e)
            {
                return CreatedAtAction(nameof(UpdateProject), new { success = false, message = e.Message});
            }
        }


        /// <summary>
        /// Deletes the project of given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the created <see cref="CreatedAtActionResult"/> of the response</returns>
        [HttpDelete(nameof(DeleteProject) + "/{id:length(24)}")]
        public async Task<ActionResult<Project>> DeleteProject(string id)
        {
            try { 
                Project project = await this.projectService.GetById(id);
                if (project is null)
                    return CreatedAtAction(nameof(DeleteProject), new { success = false, message = "Project not exists" });
                await this.projectService.DeleteProject(id);

                List<Milestone> projectMilestones = await this.milestoneService.GetByProjectId(id);
                foreach (Milestone milestone in projectMilestones)
                    await this.milestoneService.DeleteMilestone(milestone.Id);

                return CreatedAtAction(nameof(DeleteProject), new { success = true, message = "Project removed" });
            }
            catch (Exception e)
            {
                return CreatedAtAction(nameof(DeleteProject), new { success = false, message = e.Message});
            }
        }
    }
}
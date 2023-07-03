using MongoDB.Driver;
using Milestones.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Milestones_Backend.Models;
using System;

namespace Milestones.Services
{
    /// <summary>
    /// Project service class for communicate with HTTP operations.
    /// </summary>
    public class ProjectService : IProjectService
    {
        private readonly IMongoCollection<Project> projectCollection;


        /// <summary>
        /// Initializes a new instance and 
        /// initializes the instance variables,
        /// also gets the connection of MongoDB from Cloud
        /// for loading the collection
        /// </summary>
        /// <param name="databaseSettings"></param>
        public ProjectService(IOptions<DatabaseSettings> databaseSettings)
        {
            IMongoClient mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            // create collection
            if(mongoDatabase.GetCollection<Project>(nameof(Project)) == null)
                mongoDatabase.CreateCollection(nameof(Project));

            this.projectCollection = mongoDatabase.GetCollection<Project>(nameof(Project));
        }


        /// <summary>
        /// Returns the projects from the loaded collection  
        /// </summary>
        /// <returns>the projects</returns>
        public async Task<List<Project>> GetAll() =>
            await this.projectCollection.Find(project => true).ToListAsync();


        /// <summary>
        /// Returns the projects from the loaded collection by name 
        /// </summary>
        /// <returns>the projects</returns>
        public async Task<List<Project>> GetByName(string name) =>
            await this.projectCollection.Find(project => project.Name.ToLower().Contains(name.ToLower())).ToListAsync();


        /// <summary>
        /// Returns the projects started after given datetime from the loaded collection
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns>the projects</returns>
        public async Task<List<Project>> GetByStartAfter(DateTime datetime) =>
            await this.projectCollection.Find(project => project.Start >= datetime).ToListAsync();


        /// <summary>
        /// Returns the projects ended before given datetime from the loaded collection
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns>the projects</returns>
        public async Task<List<Project>> GetByEndBefore(DateTime datetime) =>
            await this.projectCollection.Find(project => project.End <= datetime).ToListAsync();


        /// <summary>
        /// Returns the project of given projectId from the loaded collection
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the project</returns>
        public async Task<Project> GetById(string id) =>
            await this.projectCollection.Find(project => project.Id == id).FirstOrDefaultAsync();


        /// <summary>
        /// Removes the project of given projectId from the loaded collection
        /// and MongoDB database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the result of delete operation</returns>
        public async Task DeleteProject(string id) =>
            await this.projectCollection.DeleteOneAsync(project => project.Id == id);


        /// <summary>
        /// Creates a new project and POST it in the MongoDB
        /// </summary>
        /// <param name="newProject"></param>
        /// <returns>the result of insert operation</returns>
        public async Task CreateProject(Project newProject) =>
            await this.projectCollection.InsertOneAsync(newProject);


        /// <summary>
        /// Updates the old project of given projectId and updatedProject instance
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedProject"></param>
        /// <returns>the result of replacement</returns>
        public async Task UpdateProject(string id, Project updatedProject) =>
            await this.projectCollection.ReplaceOneAsync(project => project.Id == id, updatedProject);
    }
}
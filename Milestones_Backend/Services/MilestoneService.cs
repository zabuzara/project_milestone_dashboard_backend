using MongoDB.Driver;
using Milestones.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System;
using Milestones_Backend.Models;

namespace Milestones.Services
{
    /// <summary>
    /// Milestone service class for communicate with HTTP operations.
    /// </summary>
    public class MilestoneService : IMilestoneService
    {
        private readonly IMongoCollection<Milestone> milestoneCollection;


        /// <summary>
        /// Initializes a new instance and 
        /// initializes the instance variables,
        /// also gets the connection of MongoDB from Cloud
        /// for loading the collection
        /// </summary>
        /// <param name="databaseSettings"></param>
        public MilestoneService(IOptions<DatabaseSettings> databaseSettings)
        {
            IMongoClient mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            // create collection
            if (mongoDatabase.GetCollection<Milestone>(nameof(Milestone)) == null)
                mongoDatabase.CreateCollection(nameof(Milestone));

            this.milestoneCollection = mongoDatabase.GetCollection<Milestone>(nameof(Milestone));
        }


        /// <summary>
        /// Returns the milestones from the loaded collection  
        /// </summary>
        /// <returns>the milestones</returns>
        public async Task<List<Milestone>> GetAll() =>
            await this.milestoneCollection.Find(milestone => true).ToListAsync();


        /// <summary>
        /// Returns the milestone of given milestoneId from the loaded collection
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the milestone</returns>
        public async Task<Milestone> GetById(string id) =>
            await this.milestoneCollection.Find(milestone => milestone.Id == id).FirstOrDefaultAsync();


        /// <summary>
        /// Returns the milestones from the loaded collection by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the milestones</returns>
        public async Task<List<Milestone>> GetByName(string name) =>
            await this.milestoneCollection.Find(milestone => milestone.Name.ToLower().Contains(name.ToLower())).ToListAsync();


        /// <summary>
        /// Returns the milestones from the loaded collection by description
        /// </summary>
        /// <param name="description"></param>
        /// <returns>the milestones</returns>
        public async Task<List<Milestone>> GetByDescription(string description) =>
            await this.milestoneCollection.Find(milestone => milestone.Description.ToLower().Contains(description.ToLower())).ToListAsync();


        /// <summary>
        /// Returns the milestones from the loaded collection by project id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the milestones</returns>
        public async Task<List<Milestone>> GetByProjectId(string id) =>
            await this.milestoneCollection.Find(milestone => milestone.ProjectReference.Equals(id)).ToListAsync();


        /// <summary>
        /// Returns the milestones from the loaded collection by member id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the milestones</returns>
        public async Task<List<Milestone>> GetByMemberId(string id)
        {
            FilterDefinition<Milestone> milestoneFilter = Builders<Milestone>.Filter.ElemMatch(milestone => milestone.Members,(member => member.Id == id));
            return await this.milestoneCollection.Find(milestoneFilter).ToListAsync();
        }


        /// <summary>
        /// Returns the milestones from the loaded collection by member firstname or lastname
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the milestones</returns>
        public async Task<List<Milestone>> GetByMemberName(string name)
        {
            FilterDefinition<Milestone> milestoneFilter = Builders<Milestone>.Filter.ElemMatch(milestone => milestone.Members, (member => member.Firstname.ToLower().Contains(name.ToLower()) || member.Lastname.ToLower().Contains(name.ToLower())));
            return await this.milestoneCollection.Find(milestoneFilter).ToListAsync();
        }


        /// <summary>
        /// Returns the milestones from the loaded collection by status
        /// </summary>
        /// <param name="status"></param>
        /// <returns>the milestones</returns>
        public async Task<List<Milestone>> GetByStatus(Status status)
        {
            return status switch
            {
                Status.COMPLETED => await this.milestoneCollection.Find(milestone => (bool)milestone.IsCompleted).ToListAsync(),
                Status.OPENS => await this.milestoneCollection.Find(milestone => !((bool)milestone.IsCompleted) && !(milestone.End < DateTime.UtcNow)).ToListAsync(),
                Status.EXPIRED => await this.milestoneCollection.Find(milestone => !((bool)milestone.IsCompleted) && milestone.End < DateTime.UtcNow).ToListAsync(),
                _ => new List<Milestone>(),
            };
        }

        /// <summary>
        /// Returns the milestones started after given datetime from the loaded collection
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns>the milestones</returns>
        public async Task<List<Milestone>> GetByStartAfter(DateTime datetime) =>
            await this.milestoneCollection.Find(milestone => milestone.Start >= datetime).ToListAsync();


        /// <summary>
        /// Returns the milestones ended before given datetime from the loaded collection
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns>the milestones</returns>
        public async Task<List<Milestone>> GetByEndBefore(DateTime datetime) =>
            await this.milestoneCollection.Find(milestone => milestone.End <= datetime).ToListAsync();


        /// <summary>
        /// Removes the milestone of given milestoneId from the loaded collection
        /// and MongoDB database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the result of delete operation</returns>
        public async Task DeleteMilestone(string id) =>
            await this.milestoneCollection.DeleteOneAsync(milestone => milestone.Id.Equals(id));


        /// <summary>
        /// Creates a new milestone and POST it in the MongoDB
        /// </summary>
        /// <param name="newMilestone"></param>
        /// <returns>the result of insert operation</returns>
        public async Task CreateMilestone(Milestone newMilestone) =>
            await this.milestoneCollection.InsertOneAsync(newMilestone);


        /// <summary>
        /// Updates the old milestone of given milestoneId and upatedMilestone instance
        /// </summary>
        /// <param name="id"></param>
        /// <param name="upatedMilestone"></param>
        /// <returns>the result of replacement</returns>
        public async Task UpdateMilestone(string id, Milestone upatedMilestone) =>
            await this.milestoneCollection.ReplaceOneAsync(milestone => milestone.Id == id, upatedMilestone);
    }
}
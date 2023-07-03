using MongoDB.Driver;
using Milestones.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Milestones.Services
{
    /// <summary>
    /// Member service class for communicate with HTTP operations.
    /// </summary>
    public class MemberService : IMemberService
    {
        private readonly IMongoCollection<Member> memberCollection;


        /// <summary>
        /// Initializes a new instance and 
        /// initializes the instance variables,
        /// also gets the connection of MongoDB from Cloud
        /// for loading the collection
        /// </summary>
        /// <param name="databaseSettings"></param>
        public MemberService(IOptions<DatabaseSettings> databaseSettings)
        {
            IMongoClient mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);

            // create collection
            if (mongoDatabase.GetCollection<Member>(nameof(Member)) == null)
                mongoDatabase.CreateCollection(nameof(Member));

            this.memberCollection = mongoDatabase.GetCollection<Member>(nameof(Member));
        }


        /// <summary>
        /// Returns the members from the loaded collection  
        /// </summary>
        /// <returns>the members</returns>
        public async Task<List<Member>> GetAll() =>
            await this.memberCollection.Find(member => true).ToListAsync();


        /// <summary>
        /// Returns the members from the loaded collection by firstname or lastname 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the members</returns>
        public async Task<List<Member>> GetByName(string name) =>
            await this.memberCollection.Find(member => member.Firstname.ToLower().Contains(name.ToLower()) || member.Lastname.ToLower().Contains(name.ToLower())).ToListAsync();


        /// <summary>
        /// Removes the member of given id from the loaded collection
        /// and MongoDB database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the result of delete operation</returns>
        public async Task DeleteMember(string id) =>
            await this.memberCollection.DeleteOneAsync(member => member.Id == id);


        /// <summary>
        /// Creates a new member and POST it in the MongoDB
        /// </summary>
        /// <param name="newMember"></param>
        /// <returns>the result of insert operation</returns>
        public async Task CreateMember(Member newMember) =>
            await this.memberCollection.InsertOneAsync(newMember);


        /// <summary>
        /// Returns the member of given id from the loaded collection
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the member</returns>
        public async Task<Member> GetById(string id) =>
            await this.memberCollection.Find(member => member.Id == id).FirstOrDefaultAsync();
    

        /// <summary>
        /// Updates the old member of given id and updatedMember instance
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedMember"></param>
        /// <returns>the result of replacement</returns>
        public async Task UpdateMember(string id, Member updatedMember) =>
            await this.memberCollection.ReplaceOneAsync(member => member.Id == id, updatedMember);
    }
}
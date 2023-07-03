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
    public class SearchService<T> : ISearchService<T>
    {
        private readonly IMongoCollection<T> collection;


        /// <summary>
        /// Initializes a new instance and 
        /// initializes the instance variables,
        /// also gets the connection of MongoDB from Cloud
        /// for loading the collection
        /// </summary>
        /// <param name="databaseSettings"></param>
        public SearchService(IOptions<DatabaseSettings> databaseSettings)
        {
            IMongoClient mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
            this.collection = mongoDatabase.GetCollection<T>(nameof(T));
        }


        public async Task<List<T>> Search(string subject) =>
            await this.collection.Find(obj => true).ToListAsync();
    }
}
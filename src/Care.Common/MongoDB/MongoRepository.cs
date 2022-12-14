using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Care.Common.MongoDB
{

    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {

        //represents the actual mongodb collection
        private readonly IMongoCollection<T> dbCollection;

        //filterbuilder to be able to query for items in mongodb
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;

        //constructor
        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<T>(collectionName);
        }

        //method to return all items in database
        public async Task<IReadOnlyCollection<T>> GetAllAsync()
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        //method to return all items from a specific filter such as retrieving all items belonging to a specific user
        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find((filter)).ToListAsync();
        }

        //method to return specific item in database
        public async Task<T> GetAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        //create (adding) new item into the database
        public async Task CreateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await dbCollection.InsertOneAsync(entity);
        }

        //update item item in database
        public async Task UpdateAsync(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            FilterDefinition<T> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        //delete item item in database
        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            await dbCollection.DeleteOneAsync(filter);
        }

        //method to return all items from a specific filter such as retrieving all items belonging to a specific user
        public async Task GetAllRemoveAsync(Expression<Func<T, bool>> filter)
        {
            await dbCollection.DeleteManyAsync(filter);
        }
    }
}
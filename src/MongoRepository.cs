using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AlbedoTeam.Sdk.DataLayerAccess.Abstractions;
using MongoDB.Driver;

namespace AlbedoTeam.Sdk.DataLayerAccess
{
    public class MongoRepository : IRepository
    {
        private static IMongoClient _client;
        private static IMongoDatabase _database;

        public MongoRepository(IDbSettings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
            _database = _client.GetDatabase(settings.DatabaseName);
        }

        public async Task<List<T>> All<T>() where T : class, new()
        {
            var list = _database.GetCollection<T>(typeof(T).Name).AsQueryable();
            return await list.ToListAsync();
        }

        public async Task<List<T>> Where<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            var list = _database.GetCollection<T>(typeof(T).Name).AsQueryable();
            return await Task.FromResult(list.Where(expression).ToList());
        }

        public async Task<T> Single<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            var list = _database.GetCollection<T>(typeof(T).Name).AsQueryable();
            return await Task.FromResult(list.Where(expression).SingleOrDefault());
        }

        public async Task Delete<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            await _database.GetCollection<T>(typeof(T).Name).DeleteManyAsync(expression);
        }

        public async Task Add<T>(T item) where T : class, new()
        {
            await _database.GetCollection<T>(typeof(T).Name).InsertOneAsync(item);
        }

        public async Task Add<T>(IEnumerable<T> items) where T : class, new()
        {
            await _database.GetCollection<T>(typeof(T).Name).InsertManyAsync(items);
        }

        public async Task ReplaceOneAsync<T>(Expression<Func<T, bool>> expression, T item)
        {
            var filter = Builders<T>.Filter.Where(expression);
            await _database.GetCollection<T>(typeof(T).Name).FindOneAndReplaceAsync(filter, item);
        }

        public async Task<bool> Exists<T>(Expression<Func<T, bool>> expression) where T : class, new()
        {
            var list = _database.GetCollection<T>(typeof(T).Name).AsQueryable();
            return await Task.FromResult(list.Any(expression));
        }
    }
}
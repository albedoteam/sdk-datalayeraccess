using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    /// <summary>
    ///     Basic interface with a few methods for adding, deleting, and querying data.
    /// </summary>
    public interface IRepository
    {
        Task<List<T>> All<T>() where T : class, new();
        Task<List<T>> Where<T>(Expression<Func<T, bool>> expression) where T : class, new();
        Task<T> Single<T>(Expression<Func<T, bool>> expression) where T : class, new();
        Task Delete<T>(Expression<Func<T, bool>> expression) where T : class, new();
        Task Add<T>(T item) where T : class, new();
        Task Add<T>(IEnumerable<T> items) where T : class, new();
        Task ReplaceOneAsync<T>(Expression<Func<T, bool>> expression, T item);
        Task<bool> Exists<T>(Expression<Func<T, bool>> expression) where T : class, new();
    }
}
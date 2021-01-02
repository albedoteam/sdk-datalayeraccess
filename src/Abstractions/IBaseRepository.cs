using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    public interface IBaseRepository<TDocument> where TDocument : IDocument
    {
        Task<IEnumerable<TDocument>> FilterBy(Expression<Func<TDocument, bool>> filterExpression);

        Task<IEnumerable<TProjected>> FilterBy<TProjected>(
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression);

        Task<TDocument> FindOne(Expression<Func<TDocument, bool>> filterExpression);

        Task<TDocument> FindById(string id);

        Task InsertOne(TDocument document);

        Task InsertMany(ICollection<TDocument> documents);

        Task DeleteById(string id);

        Task DeleteOne(Expression<Func<TDocument, bool>> filterExpression);

        Task DeleteMany(Expression<Func<TDocument, bool>> filterExpression);

        Task UpdateById(string id, UpdateDefinition<TDocument> updateDefinition);
    }
}
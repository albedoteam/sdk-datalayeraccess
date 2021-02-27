using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    public interface IBaseRepositoryWithAccount<TDocument> where TDocument : IDocumentWithAccount
    {
        IHelpersWithAccount<TDocument> Helpers { get; }

        Task<IEnumerable<TDocument>> FilterBy(
            string accountId,
            Expression<Func<TDocument, bool>> filterExpression);

        Task<(int totalPages, IReadOnlyList<TDocument> readOnlyList)> QueryByPage(
            string accountId,
            int page,
            int pageSize,
            FilterDefinition<TDocument> filterDefinition,
            SortDefinition<TDocument> sortDefinition = null);

        Task<IEnumerable<TProjected>> FilterBy<TProjected>(
            string accountId,
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression);

        Task<(int totalPages, IReadOnlyList<TProjected> readOnlyList)> QueryByPage<TProjected>(
            string accountId,
            int page,
            int pageSize,
            FilterDefinition<TDocument> filterDefinition,
            FindExpressionProjectionDefinition<TDocument, TProjected> projectionDefinition,
            SortDefinition<TDocument> sortDefinition = null);

        Task<TDocument> FindOne(string accountId, Expression<Func<TDocument, bool>> filterExpression);

        Task<TDocument> FindById(string accountId, string id, bool showDeleted = false);

        Task<TDocument> InsertOne(TDocument document);

        Task InsertMany(ICollection<TDocument> documents);

        Task DeleteById(string accountId, string id);

        Task DeleteOne(string accountId, Expression<Func<TDocument, bool>> filterExpression);

        Task DeleteMany(string accountId, Expression<Func<TDocument, bool>> filterExpression);

        Task UpdateById(
            string accountId,
            string id,
            UpdateDefinition<TDocument> updateDefinition,
            FilterDefinition<TDocument> aditionalFilter = null);
    }
}
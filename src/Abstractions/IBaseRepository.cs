namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using MongoDB.Driver;
    using Utils.Query;

    public interface IBaseRepository<TDocument> where TDocument : class, IDocument, new()
    {
        IHelpers<TDocument> Helpers { get; }

        Task<IEnumerable<TDocument>> FilterBy(Expression<Func<TDocument, bool>> filterExpression);

        Task<QueryResponse<TDocument>> QueryByPage(QueryRequest<TDocument> queryRequest);

        Task<IEnumerable<TProjected>> FilterBy<TProjected>(
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression);

        Task<(int totalPages, IReadOnlyList<TProjected> readOnlyList)> QueryByPage<TProjected>(
            int page,
            int pageSize,
            FilterDefinition<TDocument> filterDefinition,
            FindExpressionProjectionDefinition<TDocument, TProjected> projectionDefinition,
            SortDefinition<TDocument> sortDefinition = null);

        Task<TDocument> FindOne(Expression<Func<TDocument, bool>> filterExpression);

        Task<TDocument> FindById(string id,
            bool showDeleted = false,
            FilterDefinition<TDocument> aditionalFilter = null);

        Task<TDocument> InsertOne(TDocument document);

        Task InsertMany(ICollection<TDocument> documents);

        Task DeleteById(string id, FilterDefinition<TDocument> aditionalFilter = null);

        Task DeleteOne(Expression<Func<TDocument, bool>> filterExpression);

        Task DeleteMany(Expression<Func<TDocument, bool>> filterExpression);

        Task UpdateById(
            string id,
            UpdateDefinition<TDocument> updateDefinition,
            FilterDefinition<TDocument> aditionalFilter = null);
    }
}
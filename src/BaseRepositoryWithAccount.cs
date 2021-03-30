namespace AlbedoTeam.Sdk.DataLayerAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Abstractions;
    using MongoDB.Driver;
    using Utils;

    public abstract class BaseRepositoryWithAccount<TDocument> : IBaseRepositoryWithAccount<TDocument>
        where TDocument : IDocumentWithAccount
    {
        protected BaseRepositoryWithAccount(
            IBaseRepository<TDocument> baseRepository,
            IHelpersWithAccount<TDocument> helpers)
        {
            BaseRepository = baseRepository;
            Helpers = helpers;
        }

        protected IBaseRepository<TDocument> BaseRepository { get; }
        public IHelpersWithAccount<TDocument> Helpers { get; }

        public async Task<IEnumerable<TDocument>> FilterBy(
            string accountId,
            Expression<Func<TDocument, bool>> filterExpression)
        {
            var filter = filterExpression.AndAlso(a => a.AccountId == accountId);
            return await BaseRepository.FilterBy(filter);
        }

        public async Task<(int totalPages, IReadOnlyList<TDocument> readOnlyList)> QueryByPage(
            string accountId,
            int page,
            int pageSize,
            FilterDefinition<TDocument> filterDefinition,
            SortDefinition<TDocument> sortDefinition = null)
        {
            var accountFilter = Builders<TDocument>.Filter.Eq(doc => doc.AccountId, accountId);
            filterDefinition &= accountFilter;
            return await BaseRepository.QueryByPage(page, pageSize, filterDefinition, sortDefinition);
        }

        public async Task<IEnumerable<TProjected>> FilterBy<TProjected>(
            string accountId,
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression)
        {
            var filter = filterExpression.AndAlso(a => a.AccountId == accountId);
            return await BaseRepository.FilterBy(filter, projectionExpression);
        }

        public async Task<(int totalPages, IReadOnlyList<TProjected> readOnlyList)> QueryByPage<TProjected>(
            string accountId,
            int page,
            int pageSize,
            FilterDefinition<TDocument> filterDefinition,
            FindExpressionProjectionDefinition<TDocument, TProjected> projectionDefinition,
            SortDefinition<TDocument> sortDefinition = null)
        {
            var accountFilter = Builders<TDocument>.Filter.Eq(doc => doc.AccountId, accountId);
            filterDefinition &= accountFilter;

            return await BaseRepository.QueryByPage(
                page,
                pageSize,
                filterDefinition,
                projectionDefinition,
                sortDefinition);
        }

        public async Task<TDocument> FindOne(string accountId, Expression<Func<TDocument, bool>> filterExpression)
        {
            var filter = filterExpression.AndAlso(a => a.AccountId == accountId);
            return await BaseRepository.FindOne(filter);
        }

        public async Task<TDocument> FindById(string accountId, string id, bool showDeleted = false)
        {
            var accountFilter = Builders<TDocument>.Filter.Eq(doc => doc.AccountId, accountId);
            return await BaseRepository.FindById(id, showDeleted, accountFilter);
        }

        public async Task<TDocument> InsertOne(TDocument document)
        {
            return await BaseRepository.InsertOne(document);
        }

        public async Task InsertMany(ICollection<TDocument> documents)
        {
            await BaseRepository.InsertMany(documents);
        }

        public async Task DeleteById(string accountId, string id)
        {
            var accountFilter = Builders<TDocument>.Filter.Eq(doc => doc.AccountId, accountId);
            await BaseRepository.DeleteById(id, accountFilter);
        }

        public async Task DeleteOne(string accountId, Expression<Func<TDocument, bool>> filterExpression)
        {
            var filter = filterExpression.AndAlso(a => a.AccountId == accountId);
            await BaseRepository.DeleteOne(filter);
        }

        public async Task DeleteMany(string accountId, Expression<Func<TDocument, bool>> filterExpression)
        {
            var filter = filterExpression.AndAlso(a => a.AccountId == accountId);
            await BaseRepository.DeleteMany(filter);
        }

        public async Task UpdateById(string accountId, string id, UpdateDefinition<TDocument> updateDefinition,
            FilterDefinition<TDocument> aditionalFilter = null)
        {
            var accountFilter = Builders<TDocument>.Filter.Eq(doc => doc.AccountId, accountId);
            await BaseRepository.UpdateById(id, updateDefinition, accountFilter);
        }
    }
}
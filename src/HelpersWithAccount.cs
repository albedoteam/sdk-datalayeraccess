using System;
using System.Linq.Expressions;
using AlbedoTeam.Sdk.DataLayerAccess.Abstractions;
using MongoDB.Driver;

namespace AlbedoTeam.Sdk.DataLayerAccess
{
    public class HelpersWithAccount<TDocument> : IHelpersWithAccount<TDocument>
        where TDocument : IDocumentWithAccount, new()
    {
        private readonly IHelpers<TDocument> _baseHelpers;

        public HelpersWithAccount(IHelpers<TDocument> baseHelpers)
        {
            _baseHelpers = baseHelpers;
        }

        public SortDefinition<TDocument> CreateSorting(string orderBy, string sorting)
        {
            return _baseHelpers.CreateSorting(orderBy, sorting);
        }

        public FilterDefinition<TDocument> Like(Expression<Func<TDocument, object>> field, string filterBy)
        {
            return _baseHelpers.Like(field, filterBy);
        }

        public FilterDefinition<TDocument> CreateFilters(
            string accountId,
            bool showDeleted = false,
            FilterDefinition<TDocument> requiredFields = null,
            FilterDefinition<TDocument> filteredByFilters = null)
        {
            var accountFilter = Builders<TDocument>.Filter.Eq(doc => doc.AccountId, accountId);

            if (requiredFields is null)
                requiredFields = accountFilter;
            else
                requiredFields &= accountFilter;

            return _baseHelpers.CreateFilters(showDeleted, requiredFields, filteredByFilters);
        }
    }
}
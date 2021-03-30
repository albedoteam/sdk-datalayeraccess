namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    using System;
    using System.Linq.Expressions;
    using MongoDB.Driver;

    public interface IHelpersWithAccount<TDocument> where TDocument : IDocument
    {
        SortDefinition<TDocument> CreateSorting(string orderBy, string sorting);

        FilterDefinition<TDocument> Like(Expression<Func<TDocument, object>> field, string filterBy);

        FilterDefinition<TDocument> CreateFilters(
            string accountId,
            bool showDeleted = false,
            FilterDefinition<TDocument> requiredFields = null,
            FilterDefinition<TDocument> filteredByFilters = null);
    }
}
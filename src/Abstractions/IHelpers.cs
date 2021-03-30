namespace AlbedoTeam.Sdk.DataLayerAccess.Abstractions
{
    using System;
    using System.Linq.Expressions;
    using MongoDB.Driver;

    public interface IHelpers<TDocument> where TDocument : IDocument
    {
        SortDefinition<TDocument> CreateSorting(string orderBy, string sorting);

        FilterDefinition<TDocument> Like(Expression<Func<TDocument, object>> field, string filterBy);

        FilterDefinition<TDocument> CreateFilters(
            bool showDeleted = false,
            FilterDefinition<TDocument> requiredFields = null,
            FilterDefinition<TDocument> filteredByFilters = null);
    }
}
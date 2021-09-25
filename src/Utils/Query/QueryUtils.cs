namespace AlbedoTeam.Sdk.DataLayerAccess.Utils.Query
{
    using System;
    using System.Linq;
    using Abstractions;
    using FilterLanguage;
    using MongoDB.Driver;

    public static class QueryUtils
    {
        public static QueryRequest<T> GetQueryParams<T>(QueryParams parameters) where T : class, IDocument, new()
        {
            parameters ??= new QueryParams
            {
                Page = 1,
                PageSize = 1
            };

            parameters.Page = parameters.Page > 0 ? parameters.Page : 1;

            if (parameters.PageSize < 1)
                parameters.PageSize = 1;
            else if (parameters.PageSize > 100) parameters.PageSize = 100;

            var filtering = CreateFilters<T>(
                parameters.ShowDeleted,
                parameters.FilterBy);

            var ordering = CreateSorting<T>(
                parameters.OrderBy,
                parameters.Sorting);

            return new QueryRequest<T>(parameters.Page, parameters.PageSize, filtering, ordering);
        }

        private static FilterDefinition<T> CreateFilters<T>(
            bool showDeleted,
            string filterBy,
            FilterDefinition<T> requiredFields = null) where T : class, IDocument
        {
            var filteredByFilters = string.IsNullOrWhiteSpace(filterBy)
                ? null
                : FilterLanguage.ParseToFilterDefinition<T>(filterBy);

            var mainFilters = Builders<T>.Filter.And(Builders<T>.Filter.Empty);

            if (!showDeleted)
                mainFilters &= Builders<T>.Filter.Eq(a => a.IsDeleted, false);

            if (requiredFields is { })
                mainFilters &= requiredFields;

            if (filteredByFilters is { })
                mainFilters &= filteredByFilters;

            return mainFilters;
        }

        private static SortDefinition<T> CreateSorting<T>(string orderBy, string sorting)
            where T : class, IDocument, new()
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                return Builders<T>.Sort.Ascending(a => a.CreatedAt);

            var document = new T();
            var property = document
                .GetType()
                .GetProperties()
                .FirstOrDefault(p => p.Name.Equals(orderBy, StringComparison.InvariantCultureIgnoreCase));

            if (property is null)
                return Builders<T>.Sort.Ascending(a => a.CreatedAt);

            var sortBy = sorting == "Asc"
                ? Builders<T>.Sort.Ascending(new StringFieldDefinition<T>(property.Name))
                : Builders<T>.Sort.Descending(new StringFieldDefinition<T>(property.Name));

            return sortBy;
        }
    }
}
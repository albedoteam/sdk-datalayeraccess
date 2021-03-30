namespace AlbedoTeam.Sdk.DataLayerAccess.Utils
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Abstractions;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public class Helpers<TDocument> : IHelpers<TDocument> where TDocument : IDocument, new()
    {
        public SortDefinition<TDocument> CreateSorting(string orderBy, string sorting)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                return Builders<TDocument>.Sort.Ascending(a => a.CreatedAt);

            var document = new TDocument();
            var property = document
                .GetType()
                .GetProperties()
                .FirstOrDefault(p => p.Name.Equals(orderBy, StringComparison.InvariantCultureIgnoreCase));

            if (property is null)
                return Builders<TDocument>.Sort.Ascending(a => a.CreatedAt);

            var sortBy = sorting == "Asc"
                ? Builders<TDocument>.Sort.Ascending(new StringFieldDefinition<TDocument>(property.Name))
                : Builders<TDocument>.Sort.Descending(new StringFieldDefinition<TDocument>(property.Name));

            return sortBy;
        }

        public FilterDefinition<TDocument> Like(Expression<Func<TDocument, object>> field, string filterBy)
        {
            return Builders<TDocument>.Filter.Regex(field, new BsonRegularExpression(filterBy, "i"));
        }

        public FilterDefinition<TDocument> CreateFilters(
            bool showDeleted = false,
            FilterDefinition<TDocument> requiredFields = null,
            FilterDefinition<TDocument> filteredByFilters = null)
        {
            var mainFilters = Builders<TDocument>.Filter.And(Builders<TDocument>.Filter.Empty);

            if (!showDeleted)
                mainFilters &= Builders<TDocument>.Filter.Eq(a => a.IsDeleted, false);

            if (requiredFields is { })
                mainFilters &= requiredFields;

            if (filteredByFilters is { })
                mainFilters &= filteredByFilters;

            return mainFilters;
        }
    }
}
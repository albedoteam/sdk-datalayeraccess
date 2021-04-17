namespace AlbedoTeam.Sdk.DataLayerAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Abstractions;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using Utils;
    using Utils.Query;

    public abstract class BaseRepository<TDocument> : IBaseRepository<TDocument>
        where TDocument : class, IDocument, new()
    {
        protected readonly IMongoCollection<TDocument> Collection;

        protected BaseRepository(IDbContext<TDocument> context, IHelpers<TDocument> helpers)
        {
            Context = context;
            Collection = Context.GetCollection();
            Helpers = helpers;
        }

        protected IDbContext<TDocument> Context { get; }

        public IHelpers<TDocument> Helpers { get; }

        public async Task<IEnumerable<TDocument>> FilterBy(Expression<Func<TDocument, bool>> filterExpression)
        {
            var result = await Collection.FindAsync(filterExpression);
            return result.ToEnumerable();
        }

        public async Task<QueryResponse<TDocument>> QueryByPage(
            QueryRequest<TDocument> queryRequest)
        {
            var countFacet = AggregateFacet.Create("count",
                PipelineDefinition<TDocument, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<TDocument>()
                }));

            var dataFacet = AggregateFacet.Create("data",
                PipelineDefinition<TDocument, TDocument>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Sort(queryRequest.SortDefinition),
                    PipelineStageDefinitionBuilder.Skip<TDocument>((queryRequest.Page - 1) * queryRequest.PageSize),
                    PipelineStageDefinitionBuilder.Limit<TDocument>(queryRequest.PageSize)
                }));

            // turning case insentive for indexes (find and sort) .. indexes needs to be created at mongodb
            var options = new AggregateOptions
            {
                Collation = new Collation("en", strength: CollationStrength.Secondary)
            };

            var aggregation = await Collection.Aggregate(options)
                .Match(queryRequest.FilterDefinition)
                .Facet(countFacet, dataFacet)
                .ToListAsync();

            var facetResults = aggregation.FirstOrDefault();
            if (facetResults is null)
                return new QueryResponse<TDocument>(0, 0, 0, null);

            var countFacetResult = facetResults.Facets.FirstOrDefault(f => f.Name == "count");
            if (countFacetResult is null)
                return new QueryResponse<TDocument>(0, 0, 0, null);

            var countResult = countFacetResult
                .Output<AggregateCountResult>()
                .FirstOrDefault();

            if (countResult is null)
                return new QueryResponse<TDocument>(0, 0, 0, null);

            var count = countResult.Count;

            var rest = count % queryRequest.PageSize;
            var totalPages = (int) count / queryRequest.PageSize;
            if (rest > 0) totalPages += 1;

            var dataFacetResults = facetResults.Facets.FirstOrDefault(x => x.Name == "data");
            if (dataFacetResults is null)
                return new QueryResponse<TDocument>(0, 0, 0, null);

            var data = dataFacetResults.Output<TDocument>();
            return new QueryResponse<TDocument>(queryRequest.Page, queryRequest.PageSize, totalPages, data);
        }

        public async Task<IEnumerable<TProjected>> FilterBy<TProjected>(
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression)
        {
            var findOptions = new FindOptions<TDocument, TProjected>
            {
                Projection = new FindExpressionProjectionDefinition<TDocument, TProjected>(projectionExpression)
            };

            var result = await Collection.FindAsync(filterExpression, findOptions);

            return result.ToEnumerable();
        }

        public async Task<(int totalPages, IReadOnlyList<TProjected> readOnlyList)> QueryByPage<TProjected>(
            int page,
            int pageSize,
            FilterDefinition<TDocument> filterDefinition,
            FindExpressionProjectionDefinition<TDocument, TProjected> projectionDefinition,
            SortDefinition<TDocument> sortDefinition = null)
        {
            var countFacet = AggregateFacet.Create("count",
                PipelineDefinition<TProjected, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<TDocument>()
                }));

            var dataFacet = AggregateFacet.Create("data",
                PipelineDefinition<TProjected, TProjected>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Sort(sortDefinition),
                    PipelineStageDefinitionBuilder.Skip<TDocument>((page - 1) * pageSize),
                    PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize)
                }));

            // turning case insentive for indexes (find and sort) .. indexes needs to be created at mongodb
            var options = new AggregateOptions
            {
                Collation = new Collation("en", strength: CollationStrength.Secondary)
            };

            var aggregation = await Collection.Aggregate(options)
                .Match(filterDefinition)
                .Project(projectionDefinition)
                .Facet(countFacet, dataFacet)
                .ToListAsync();

            var facetResults = aggregation.FirstOrDefault();
            if (facetResults is null)
                return (0, new List<TProjected>());

            var countFacetResult = facetResults.Facets.FirstOrDefault(f => f.Name == "count");
            if (countFacetResult is null)
                return (0, new List<TProjected>());

            var countResult = countFacetResult
                .Output<AggregateCountResult>()
                .FirstOrDefault();

            if (countResult is null)
                return (0, new List<TProjected>());

            var count = countResult.Count;

            var rest = count % pageSize;
            var totalPages = (int) count / pageSize;
            if (rest > 0) totalPages += 1;

            var dataFacetResults = facetResults.Facets.FirstOrDefault(x => x.Name == "data");
            if (dataFacetResults is null)
                return (0, new List<TProjected>());

            var data = dataFacetResults.Output<TProjected>();
            return (totalPages, data);
        }

        public async Task<TDocument> FindOne(Expression<Func<TDocument, bool>> filterExpression)
        {
            return (await Collection.FindAsync(filterExpression)).FirstOrDefault();
        }

        public async Task<TDocument> FindById(
            string id,
            bool showDeleted,
            FilterDefinition<TDocument> aditionalFilter = null)
        {
            var objectId = new ObjectId(id);

            FilterDefinition<TDocument> filter;
            if (showDeleted)
                filter = Builders<TDocument>.Filter.And(
                    Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId),
                    Builders<TDocument>.Filter.Or(
                        Builders<TDocument>.Filter.Eq(doc => doc.IsDeleted, false),
                        Builders<TDocument>.Filter.Eq(doc => doc.IsDeleted, true)));
            else
                filter = Builders<TDocument>.Filter.And(
                    Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId),
                    Builders<TDocument>.Filter.Eq(doc => doc.IsDeleted, false));

            if (aditionalFilter is { })
                filter &= aditionalFilter;

            return (await Collection.FindAsync(filter)).SingleOrDefault();
        }

        public async Task<TDocument> InsertOne(TDocument document)
        {
            if (document == null) throw new ArgumentNullException(typeof(TDocument).Name + " object is null");

            await Collection.InsertOneAsync(document);
            return document;
        }

        public async Task InsertMany(ICollection<TDocument> documents)
        {
            if (documents == null)
                throw new ArgumentNullException(typeof(TDocument).Name + " object collection is null");

            await Collection.InsertManyAsync(documents);
        }

        public async Task DeleteById(string id, FilterDefinition<TDocument> aditionalFilter = null)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);

            var update = Builders<TDocument>.Update.Combine(
                Builders<TDocument>.Update.Set(d => d.IsDeleted, true),
                Builders<TDocument>.Update.Set(d => d.DeletedAt, DateTime.Now));

            await Collection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteOne(Expression<Func<TDocument, bool>> filterExpression)
        {
            if (filterExpression == null) throw new ArgumentNullException(nameof(filterExpression));

            var update = Builders<TDocument>.Update.Combine(
                Builders<TDocument>.Update.Set(d => d.IsDeleted, true),
                Builders<TDocument>.Update.Set(d => d.DeletedAt, DateTime.Now));

            await Collection.UpdateOneAsync(filterExpression, update);
        }

        public async Task DeleteMany(Expression<Func<TDocument, bool>> filterExpression)
        {
            if (filterExpression == null) throw new ArgumentNullException(nameof(filterExpression));

            var update = Builders<TDocument>.Update.Combine(
                Builders<TDocument>.Update.Set(d => d.IsDeleted, true),
                Builders<TDocument>.Update.Set(d => d.DeletedAt, DateTime.Now));

            await Collection.UpdateManyAsync(filterExpression, update);
        }

        public async Task UpdateById(string id, UpdateDefinition<TDocument> updateDefinition,
            FilterDefinition<TDocument> aditionalFilter = null)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);

            if (updateDefinition == null) throw new ArgumentNullException(nameof(updateDefinition));

            updateDefinition = updateDefinition.Set(doc => doc.UpdatedAt, DateTime.Now);

            await Collection.UpdateOneAsync(filter, updateDefinition);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AlbedoTeam.Sdk.DataLayerAccess.Abstractions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AlbedoTeam.Sdk.DataLayerAccess
{
    public abstract class BaseRepository<TDocument> : IBaseRepository<TDocument> where TDocument : IDocument
    {
        private readonly IMongoCollection<TDocument> _collection;

        protected BaseRepository(IDbContext<TDocument> context)
        {
            Context = context;
            _collection = Context.GetCollection();
        }

        protected IDbContext<TDocument> Context { get; }

        public async Task<IEnumerable<TDocument>> FilterBy(Expression<Func<TDocument, bool>> filterExpression)
        {
            var result = await _collection.FindAsync(filterExpression);
            return result.ToEnumerable();
        }

        public async Task<(int totalPages, IReadOnlyList<TDocument> readOnlyList)> QueryByPage(
            int page, 
            int pageSize,
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, object>> sortExpression)
        {
            var countFacet = AggregateFacet.Create("count",
                PipelineDefinition<TDocument, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<TDocument>()
                }));
            
            var dataFacet = AggregateFacet.Create("data",
                PipelineDefinition<TDocument, TDocument>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Sort(Builders<TDocument>.Sort.Ascending(sortExpression)),
                    PipelineStageDefinitionBuilder.Skip<TDocument>((page - 1) * pageSize),
                    PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize),
                }));
            
            var aggregation = await _collection.Aggregate()
                .Match(filterExpression)
                .Facet(countFacet, dataFacet)
                .ToListAsync();
            
            var count = aggregation.First()
                .Facets.First(x => x.Name == "count")
                .Output<AggregateCountResult>()
                .First()
                .Count;
            
            var rest = (count % pageSize);
            var totalPages = (int)count / pageSize;
            if (rest > 0) totalPages += 1;
            
            var data = aggregation.First()
                .Facets.First(x => x.Name == "data")
                .Output<TDocument>();
            
            return (totalPages, data);
        }

        public async Task<IEnumerable<TProjected>> FilterBy<TProjected>(
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression)
        {
            var findOptions = new FindOptions<TDocument, TProjected>
            {
                Projection = new FindExpressionProjectionDefinition<TDocument, TProjected>(projectionExpression)
            };

            var result = await _collection.FindAsync(filterExpression, findOptions);

            return result.ToEnumerable();
        }

        public async Task<(int totalPages, IReadOnlyList<TProjected> readOnlyList)> QueryByPage<TProjected>(
            int page,
            int pageSize, 
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression,
            Expression<Func<TDocument, object>> sortExpression)
        {
            var projection = new FindExpressionProjectionDefinition<TDocument, TProjected>(projectionExpression);
            
            var countFacet = AggregateFacet.Create("count",
                PipelineDefinition<TProjected, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<TDocument>()
                }));
            
            var dataFacet = AggregateFacet.Create("data",
                PipelineDefinition<TProjected, TProjected>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Sort(Builders<TDocument>.Sort.Ascending(sortExpression)),
                    PipelineStageDefinitionBuilder.Skip<TDocument>((page - 1) * pageSize),
                    PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize),
                }));
            
            var aggregation = await _collection.Aggregate()
                .Match(filterExpression)
                .Project(projection)
                .Facet(countFacet, dataFacet)
                .ToListAsync();
            
            var count = aggregation.First()
                .Facets.First(x => x.Name == "count")
                .Output<AggregateCountResult>()
                .First()
                .Count;
            
            var rest = (count % pageSize);
            var totalPages = (int)count / pageSize;
            if (rest > 0) totalPages += 1;
            
            var data = aggregation.First()
                .Facets.First(x => x.Name == "data")
                .Output<TProjected>();
            
            return (totalPages, data);
        }

        public async Task<TDocument> FindOne(Expression<Func<TDocument, bool>> filterExpression)
        {
            return (await _collection.FindAsync(filterExpression)).FirstOrDefault();
        }

        public async Task<TDocument> FindById(string id, bool showDeleted)
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

            return (await _collection.FindAsync(filter)).SingleOrDefault();
        }

        public async Task<TDocument> InsertOne(TDocument document)
        {
            if (document == null) throw new ArgumentNullException(typeof(TDocument).Name + " object is null");

            await _collection.InsertOneAsync(document);
            return document;
        }

        public async Task InsertMany(ICollection<TDocument> documents)
        {
            if (documents == null)
                throw new ArgumentNullException(typeof(TDocument).Name + " object collection is null");

            await _collection.InsertManyAsync(documents);
        }

        public async Task DeleteById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);

            var update = Builders<TDocument>.Update.Combine(
                Builders<TDocument>.Update.Set(d => d.IsDeleted, true),
                Builders<TDocument>.Update.Set(d => d.DeletedAt, DateTime.Now));

            await _collection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteOne(Expression<Func<TDocument, bool>> filterExpression)
        {
            if (filterExpression == null) throw new ArgumentNullException(nameof(filterExpression));

            var update = Builders<TDocument>.Update.Combine(
                Builders<TDocument>.Update.Set(d => d.IsDeleted, true),
                Builders<TDocument>.Update.Set(d => d.DeletedAt, DateTime.Now));

            await _collection.UpdateOneAsync(filterExpression, update);
        }

        public async Task DeleteMany(Expression<Func<TDocument, bool>> filterExpression)
        {
            if (filterExpression == null) throw new ArgumentNullException(nameof(filterExpression));

            var update = Builders<TDocument>.Update.Combine(
                Builders<TDocument>.Update.Set(d => d.IsDeleted, true),
                Builders<TDocument>.Update.Set(d => d.DeletedAt, DateTime.Now));

            await _collection.UpdateManyAsync(filterExpression, update);
        }

        public async Task UpdateById(string id, UpdateDefinition<TDocument> updateDefinition)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);

            if (updateDefinition == null) throw new ArgumentNullException(nameof(updateDefinition));

            updateDefinition = updateDefinition.Set(doc => doc.UpdatedAt, DateTime.Now);

            await _collection.UpdateOneAsync(filter, updateDefinition);
        }
    }
}
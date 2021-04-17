namespace AlbedoTeam.Sdk.DataLayerAccess.Utils.Query
{
    using Abstractions;
    using MongoDB.Driver;

    public struct QueryRequest<T> where T : class, IDocument
    {
        public QueryRequest(
            int page,
            int pageSize,
            FilterDefinition<T> filterDefinition,
            SortDefinition<T> sortDefinition = null)
        {
            Page = page;
            PageSize = pageSize;
            FilterDefinition = filterDefinition;
            SortDefinition = sortDefinition;
        }

        public int Page { get; }
        public int PageSize { get; }
        public FilterDefinition<T> FilterDefinition { get; set; }
        public SortDefinition<T> SortDefinition { get; }
    }
}
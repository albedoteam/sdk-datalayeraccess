namespace AlbedoTeam.Sdk.DataLayerAccess.Utils.Query
{
    using System.Collections.Generic;
    using Abstractions;

    public struct QueryResponse<T> where T : class, IDocument
    {
        public QueryResponse(int page, int pageSize, int totalPages, IReadOnlyList<T> records)
        {
            Page = page;
            PageSize = pageSize;
            TotalPages = totalPages;
            Records = records;
        }

        public int Page { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public IReadOnlyList<T> Records { get; }
        public int RecordsInPage => Records?.Count ?? 0;
    }
}
namespace AlbedoTeam.Sdk.DataLayerAccess.Utils.Query
{
    public class QueryParams
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool ShowDeleted { get; set; }
        public string FilterBy { get; set; }
        public string OrderBy { get; set; }
        public string Sorting { get; set; }
    }
}
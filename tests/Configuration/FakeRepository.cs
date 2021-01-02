using AlbedoTeam.Sdk.DataLayerAccess.Abstractions;

namespace AlbedoTeam.Sdk.DataLayerAccess.Tests.Configuration
{
    public class FakeRepository : BaseRepository<FakeDocument>
    {
        public FakeRepository(IDbContext<FakeDocument> context) : base(context)
        {
        }
    }
}
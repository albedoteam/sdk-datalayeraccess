namespace AlbedoTeam.Sdk.DataLayerAccess.Tests
{
    using Abstractions;
    using Configuration;
    using MongoDB.Driver;
    using Moq;
    using Xunit;

    public class MongoDbContextTests
    {
        private readonly Mock<IMongoClient> _mockClient;
        private readonly Mock<IMongoDatabase> _mockDb;
        private readonly Mock<IDbSettings> _mockOptions;

        public MongoDbContextTests()
        {
            _mockOptions = new Mock<IDbSettings>();
            _mockDb = new Mock<IMongoDatabase>();
            _mockClient = new Mock<IMongoClient>();
        }

        [Fact]
        public void MongoDBContext_Constructor_Success()
        {
            _mockOptions.Setup(s => s.ConnectionString).Returns("mongodb://tes123");
            _mockOptions.Setup(s => s.DatabaseName).Returns("TestDb");

            _mockClient.Setup(c => c.GetDatabase(_mockOptions.Object.DatabaseName, null))
                .Returns(_mockDb.Object);

            //Act 
            var context = new DbContext<FakeDocument>(_mockOptions.Object);

            //Assert 
            Assert.NotNull(context);
        }

        [Fact]
        public void MongoDBContext_GetCollection_WithAttr_Success()
        {
            _mockOptions.Setup(s => s.ConnectionString).Returns("mongodb://tes123");
            _mockOptions.Setup(s => s.DatabaseName).Returns("TestDb");

            _mockClient.Setup(c => c.GetDatabase(_mockOptions.Object.DatabaseName, null))
                .Returns(_mockDb.Object);

            //Act 
            var collection = new DbContext<FakeDocument>(_mockOptions.Object).GetCollection();

            //Assert 
            Assert.Equal("fakeDocs", collection.CollectionNamespace.CollectionName);
        }

        [Fact]
        public void MongoDBContext_GetCollection_WithoutAttr_Failure()
        {
            _mockOptions.Setup(s => s.ConnectionString).Returns("mongodb://tes123");
            _mockOptions.Setup(s => s.DatabaseName).Returns("TestDb");

            _mockClient.Setup(c => c.GetDatabase(_mockOptions.Object.DatabaseName, null))
                .Returns(_mockDb.Object);

            //Act 
            var collection = new DbContext<FakeDocumentWithoutAttr>(_mockOptions.Object).GetCollection();

            //Assert 
            Assert.Null(collection);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AlbedoTeam.Sdk.DataLayerAccess.Abstractions;
using AlbedoTeam.Sdk.DataLayerAccess.Tests.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace AlbedoTeam.Sdk.DataLayerAccess.Tests
{
    public class BaseRepositoryTests
    {
        private readonly Mock<IAsyncCursor<FakeDocument>> _fakeCursor;
        private readonly Mock<IMongoCollection<FakeDocument>> _mockCollection;
        private readonly Mock<IDbContext<FakeDocument>> _mockContext;
        private readonly List<FakeDocumentProjection> _projectionList;
        private FakeDocument _fakeDoc;

        public BaseRepositoryTests()
        {
            _mockCollection = new Mock<IMongoCollection<FakeDocument>>();
            _mockContext = new Mock<IDbContext<FakeDocument>>();
            _fakeDoc = new FakeDocument {Id = ObjectId.GenerateNewId(), Name = "A", Text = "TA", Active = true};
            var list = new List<FakeDocument> {_fakeDoc};

            //Mock MoveNextAsync
            _fakeCursor = new Mock<IAsyncCursor<FakeDocument>>();
            _fakeCursor.Setup(_ => _.Current).Returns(list);
            _fakeCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            _fakeCursor
                .SetupSequence(_ => _.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            _projectionList = new List<FakeDocumentProjection>
            {
                new FakeDocumentProjection
                {
                    Id = _fakeDoc.Id,
                    Name = _fakeDoc.Name,
                    Active = _fakeDoc.Active
                }
            };
        }

        [Fact]
        public async void FindById_ValidId_Success()
        {
            //Arrange

            //Mock FindAsync
            _mockCollection.Setup(op => op.FindAsync(It.IsAny<FilterDefinition<FakeDocument>>(),
                It.IsAny<FindOptions<FakeDocument, FakeDocument>>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(_fakeCursor.Object);

            //Mock GetCollection
            _mockContext.Setup(c => c.GetCollection()).Returns(_mockCollection.Object);

            var bookRepo = new FakeBaseRepository(_mockContext.Object);

            //Act
            var result = await bookRepo.FindById(_fakeDoc.Id.ToString(), false);

            //Assert 
            Assert.NotNull(result);
            Assert.IsType<FakeDocument>(result);

            //Verify if InsertOneAsync is called once 
            _mockCollection.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<FakeDocument>>(),
                It.IsAny<FindOptions<FakeDocument>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void FindById_InvalidId_Failure()
        {
            //Arrange
            //Mock GetCollection
            _mockContext.Setup(c => c.GetCollection()).Returns(_mockCollection.Object);

            var bookRepo = new FakeBaseRepository(_mockContext.Object);

            //Act
            //Assert 
            await Assert.ThrowsAsync<IndexOutOfRangeException>(() => bookRepo.FindById("123", false));
        }

        [Fact]
        public async void FindOne_Success()
        {
            //Arrange

            //Mock FindAsync
            _mockCollection.Setup(op => op.FindAsync(It.IsAny<FilterDefinition<FakeDocument>>(),
                It.IsAny<FindOptions<FakeDocument, FakeDocument>>(),
                It.IsAny<CancellationToken>())).Returns(Task.FromResult(_fakeCursor.Object));

            //Mock GetCollection
            _mockContext.Setup(c => c.GetCollection()).Returns(_mockCollection.Object);

            var fakeRepo = new FakeBaseRepository(_mockContext.Object);

            //Act
            var result = await fakeRepo.FindOne(f => !f.Active);

            //Assert 
            Assert.NotNull(result);
            Assert.IsType<FakeDocument>(result);

            //Verify if FindOne is called once 
            _mockCollection.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<FakeDocument>>(),
                It.IsAny<FindOptions<FakeDocument>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void InsertOne_Valid_Success()
        {
            _mockCollection.Setup(op => op.InsertOneAsync(_fakeDoc, null, default)).Returns(Task.CompletedTask);

            _mockContext.Setup(c => c.GetCollection()).Returns(_mockCollection.Object);
            var fakeRepo = new FakeBaseRepository(_mockContext.Object);

            //Act
            await fakeRepo.InsertOne(_fakeDoc);

            //Assert 

            //Verify if InsertOneAsync is called once 
            _mockCollection.Verify(c => c.InsertOneAsync(_fakeDoc, null, default), Times.Once);
        }

        [Fact]
        public async void InsertOne_Null_Failure()
        {
            // Arrange
            _fakeDoc = null;

            //Act 
            var bookRepo = new FakeBaseRepository(_mockContext.Object);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => bookRepo.InsertOne(_fakeDoc));
        }

        [Fact]
        public async void FilterBy_FilterNull_Failure()
        {
            //Arrange

            //Mock FindAsync
            _mockCollection.Setup(op => op.FindAsync(It.IsAny<FilterDefinition<FakeDocument>>(),
                It.IsAny<FindOptions<FakeDocument, FakeDocument>>(),
                It.IsAny<CancellationToken>())).Returns(Task.FromResult(_fakeCursor.Object));

            //Mock GetCollection
            _mockContext.Setup(c => c.GetCollection()).Returns(_mockCollection.Object);

            var fakeRepo = new FakeBaseRepository(_mockContext.Object);

            //Act

            //Assert 
            await Assert.ThrowsAsync<ArgumentNullException>(() => fakeRepo.FilterBy(null));
        }

        [Fact]
        public async void FilterBy_Success()
        {
            //Arrange

            //Mock FindAsync
            _mockCollection.Setup(op => op.FindAsync(It.IsAny<FilterDefinition<FakeDocument>>(),
                It.IsAny<FindOptions<FakeDocument, FakeDocument>>(),
                It.IsAny<CancellationToken>())).Returns(Task.FromResult(_fakeCursor.Object));

            //Mock GetCollection
            _mockContext.Setup(c => c.GetCollection()).Returns(_mockCollection.Object);

            var fakeRepo = new FakeBaseRepository(_mockContext.Object);

            //Act
            var result = await fakeRepo.FilterBy(f => true);

            //Assert 
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<FakeDocument>>(result);

            //loop only first item and assert
            foreach (var fakeDoc in result)
            {
                Assert.Equal(fakeDoc.Id, _fakeDoc.Id);
                break;
            }

            //Verify if FilterBy is called once 
            _mockCollection.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<FakeDocument>>(),
                It.IsAny<FindOptions<FakeDocument>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void FilterBy_Projection_Success()
        {
            //Arrange

            //Mock MoveNextAsync
            var fakeCursor = new Mock<IAsyncCursor<FakeDocumentProjection>>();
            fakeCursor.Setup(_ => _.Current).Returns(_projectionList);
            fakeCursor
                .SetupSequence(_ => _.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);

            //Mock FindAsync
            _mockCollection.Setup(op => op.FindAsync(It.IsAny<FilterDefinition<FakeDocument>>(),
                It.IsAny<FindOptions<FakeDocument, FakeDocumentProjection>>(),
                It.IsAny<CancellationToken>())).Returns(Task.FromResult(fakeCursor.Object));

            //Mock GetCollection
            _mockContext.Setup(c => c.GetCollection()).Returns(_mockCollection.Object);

            var fakeRepo = new FakeBaseRepository(_mockContext.Object);

            //Act
            var result = await fakeRepo.FilterBy(f => f.Id == _fakeDoc.Id, p => new FakeDocumentProjection
            {
                Id = p.Id,
                Name = p.Name,
                Active = p.Active
            });

            //Assert 
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<FakeDocumentProjection>>(result);

            //loop only first item and assert
            foreach (var fakeDoc in result)
            {
                Assert.NotNull(fakeDoc);
                Assert.Equal(fakeDoc.Id, _fakeDoc.Id);
                Assert.Equal(fakeDoc.Name, _fakeDoc.Name);
                Assert.Equal(fakeDoc.Active, _fakeDoc.Active);
                break;
            }

            //Verify if FilterBy is called once 
            _mockCollection.Verify(c => c.FindAsync(It.IsAny<FilterDefinition<FakeDocument>>(),
                It.IsAny<FindOptions<FakeDocument, FakeDocumentProjection>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void InsertMany_Valid_Success()
        {
            var list = new List<FakeDocument> {_fakeDoc};

            _mockCollection.Setup(op => op.InsertManyAsync(list, null, default))
                .Returns(Task.CompletedTask);

            _mockContext.Setup(c => c.GetCollection()).Returns(_mockCollection.Object);
            var fakeRepo = new FakeBaseRepository(_mockContext.Object);

            //Act
            await fakeRepo.InsertMany(list);

            //Assert 
            //Verify if InsertOneAsync is called once 
            _mockCollection.Verify(c => c.InsertManyAsync(list, null, default), Times.Once);
        }

        [Fact]
        public async void InsertMany_Null_Failure()
        {
            // Arrange
            //Act 
            var bookRepo = new FakeBaseRepository(_mockContext.Object);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => bookRepo.InsertMany(null));
        }

        [Fact]
        public async void DeleteOne_Valid_Success()
        {
            var mockUpdateResult = new Mock<UpdateResult>();
            //Set up the mocks behavior
            mockUpdateResult.Setup(_ => _.IsAcknowledged).Returns(true);
            mockUpdateResult.Setup(_ => _.ModifiedCount).Returns(1);

            _mockCollection.Setup(op => op.UpdateOneAsync(
                    It.IsAny<FilterDefinition<FakeDocument>>(),
                    It.IsAny<UpdateDefinition<FakeDocument>>(),
                    It.IsAny<UpdateOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockUpdateResult.Object);

            _mockContext.Setup(c => c.GetCollection()).Returns(_mockCollection.Object);
            var fakeRepo = new FakeBaseRepository(_mockContext.Object);

            //Act
            await fakeRepo.DeleteOne(f => f.Id == ObjectId.GenerateNewId());

            //Assert 
            //Verify if InsertOneAsync is called once 
            _mockCollection.Verify(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<FakeDocument>>(),
                It.IsAny<UpdateDefinition<FakeDocument>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void DeleteOne_Invalid_Failure()
        {
            // Arrange
            //Act 
            var bookRepo = new FakeBaseRepository(_mockContext.Object);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => bookRepo.DeleteOne(null));
        }

        [Fact]
        public async void DeleteById_Valid_Failure()
        {
            var mockUpdateResult = new Mock<UpdateResult>();
            //Set up the mocks behavior
            mockUpdateResult.Setup(_ => _.IsAcknowledged).Returns(true);
            mockUpdateResult.Setup(_ => _.ModifiedCount).Returns(1);

            _mockCollection.Setup(op => op.UpdateOneAsync(
                    It.IsAny<FilterDefinition<FakeDocument>>(),
                    It.IsAny<UpdateDefinition<FakeDocument>>(),
                    It.IsAny<UpdateOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockUpdateResult.Object);

            _mockContext.Setup(c => c.GetCollection()).Returns(_mockCollection.Object);
            var fakeRepo = new FakeBaseRepository(_mockContext.Object);

            //Act
            await fakeRepo.DeleteById(_fakeDoc.Id.ToString());

            //Assert 
            //Verify if InsertOneAsync is called once 
            _mockCollection.Verify(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<FakeDocument>>(),
                It.IsAny<UpdateDefinition<FakeDocument>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void DeleteById_InvalidId_Failure()
        {
            //Arrange
            //Mock GetCollection
            _mockContext.Setup(c => c.GetCollection()).Returns(_mockCollection.Object);

            var bookRepo = new FakeBaseRepository(_mockContext.Object);

            //Act
            //Assert 
            await Assert.ThrowsAsync<IndexOutOfRangeException>(() => bookRepo.DeleteById("123"));
        }

        [Fact]
        public async void DeleteMany_Valid_Success()
        {
            var mockUpdateResult = new Mock<UpdateResult>();
            //Set up the mocks behavior
            mockUpdateResult.Setup(_ => _.IsAcknowledged).Returns(true);
            mockUpdateResult.Setup(_ => _.ModifiedCount).Returns(1);

            _mockCollection.Setup(op => op.UpdateManyAsync(
                    It.IsAny<FilterDefinition<FakeDocument>>(),
                    It.IsAny<UpdateDefinition<FakeDocument>>(),
                    It.IsAny<UpdateOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockUpdateResult.Object);

            _mockContext.Setup(c => c.GetCollection()).Returns(_mockCollection.Object);
            var fakeRepo = new FakeBaseRepository(_mockContext.Object);

            //Act
            await fakeRepo.DeleteMany(f => !f.Active);

            //Assert 
            //Verify if InsertOneAsync is called once 
            _mockCollection.Verify(c => c.UpdateManyAsync(
                It.IsAny<FilterDefinition<FakeDocument>>(),
                It.IsAny<UpdateDefinition<FakeDocument>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void DeleteMany_Invalid_Failure()
        {
            // Arrange
            //Act 
            var bookRepo = new FakeBaseRepository(_mockContext.Object);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => bookRepo.DeleteMany(null));
        }

        [Fact]
        public async void UpdateById_Valid_Success()
        {
            var mockUpdateResult = new Mock<UpdateResult>();
            //Set up the mocks behavior
            mockUpdateResult.Setup(_ => _.IsAcknowledged).Returns(true);
            mockUpdateResult.Setup(_ => _.ModifiedCount).Returns(1);

            _mockCollection.Setup(op => op.UpdateOneAsync(
                    It.IsAny<FilterDefinition<FakeDocument>>(),
                    It.IsAny<UpdateDefinition<FakeDocument>>(),
                    It.IsAny<UpdateOptions>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockUpdateResult.Object);

            _mockContext.Setup(c => c.GetCollection()).Returns(_mockCollection.Object);
            var fakeRepo = new FakeBaseRepository(_mockContext.Object);

            var updateDefinition = Builders<FakeDocument>.Update.Combine(
                Builders<FakeDocument>.Update.Set(d => d.Name, "Novo nome"),
                Builders<FakeDocument>.Update.Set(d => d.Active, false));

            //Act
            await fakeRepo.UpdateById(_fakeDoc.Id.ToString(), updateDefinition);

            //Assert 
            //Verify if InsertOneAsync is called once 
            _mockCollection.Verify(c => c.UpdateOneAsync(
                It.IsAny<FilterDefinition<FakeDocument>>(),
                It.IsAny<UpdateDefinition<FakeDocument>>(),
                It.IsAny<UpdateOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
namespace AIC.Core.Data.CosmosDb.Tests.Implementations
{
    using NUnit.Framework;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AIC.Core.Data.CosmosDb.Implementations;
    using Microsoft.Azure.CosmosRepository;

    [TestFixture]
    public class BaseCosmosDbRepositoryTests
    {
        private Mock<IRepository<TestModel>> cosmosRepositoryMock;
        private BaseCosmosDbRepository<TestModel> repository;

        [SetUp]
        public void SetUp()
        {
            this.cosmosRepositoryMock = new Mock<IRepository<TestModel>>();
            this.repository = new TestCosmosDbRepository(this.cosmosRepositoryMock.Object);
        }

        [Test]
        public async Task CreateOrUpdateAsync_ShouldCreateModel_WhenModelDoesNotExist()
        {
            // Arrange
            var model = new TestModel { Id = Guid.NewGuid().ToString(), Name = "TestModel" };

            this.cosmosRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None)).ReturnsAsync((TestModel?)null);
            this.cosmosRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<TestModel>(), CancellationToken.None)).ReturnsAsync(model);

            // Act
            var result = await this.repository.CreateOrUpdateAsync(model);

            // Assert
            this.cosmosRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<TestModel>(), CancellationToken.None), Times.Once);
            Assert.AreEqual(model, result);
        }

        [Test]
        public async Task CreateOrUpdateAsync_ShouldReturnExistingModel_WhenModelExists()
        {
            // Arrange
            var model = new TestModel { Id = Guid.NewGuid().ToString(), Name = "TestModel" };

            this.cosmosRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(model);

            // Act
            var result = await this.repository.CreateOrUpdateAsync(model);

            // Assert
            this.cosmosRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<TestModel>(), CancellationToken.None), Times.Never);
            Assert.AreEqual(model, result);
        }

        [Test]
        public async Task CreateOrUpdateAsync_ShouldCreateMultipleModels_WhenTheyDoNotExist()
        {
            // Arrange
            var models = new List<TestModel>
        {
            new TestModel { Id = Guid.NewGuid().ToString(), Name = "TestModel1" },
            new TestModel { Id = Guid.NewGuid().ToString(), Name = "TestModel2" }
        };

            this.cosmosRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None)).ReturnsAsync((TestModel)null);
            this.cosmosRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<TestModel>(), CancellationToken.None)).ReturnsAsync((TestModel model) => model);

            // Act
            var result = await this.repository.CreateOrUpdateAsync(models);

            // Assert
            this.cosmosRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<TestModel>(), CancellationToken.None), Times.Exactly(models.Count));
            Assert.AreEqual(models.Count, result.Count());
        }

        [Test]
        public async Task GetModelAsync_ShouldReturnModel_WhenModelExists()
        {
            // Arrange
            var model = new TestModel { Id = Guid.NewGuid().ToString(), Name = "TestModel" };

            this.cosmosRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(model);

            // Act
            var result = await this.repository.GetModelAsync(Guid.Parse(model.Id));

            // Assert
            Assert.AreEqual(model, result);
        }

        [Test]
        public async Task GetModelAsync_ShouldReturnNull_WhenModelDoesNotExist()
        {
            // Arrange
            this.cosmosRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None)).ReturnsAsync((TestModel)null);

            // Act
            var result = await this.repository.GetModelAsync(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetModelsAsync_ShouldReturnAllModels()
        {
            // Arrange
            var models = new List<TestModel> { new TestModel { Id = Guid.NewGuid().ToString(), Name = "TestModel1" }, new TestModel { Id = Guid.NewGuid().ToString(), Name = "TestModel2" } };

            this.cosmosRepositoryMock.Setup(repo => repo.GetByQueryAsync(It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(models);

            // Act
            var result = await this.repository.GetModelsAsync();

            // Assert
            Assert.AreEqual(models.Count, result.Count());
        }

        [Test]
        public async Task DeleteAsync_ShouldDeleteModel_WhenModelExists()
        {
            // Arrange
            var model = new TestModel { Id = Guid.NewGuid().ToString(), Name = "TestModel" };

            this.cosmosRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None)).ReturnsAsync(model);
            this.cosmosRepositoryMock.Setup(repo => repo.DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None)).Returns(ValueTask.CompletedTask);

            // Act
            var result = await this.repository.DeleteAsync(Guid.Parse(model.Id));

            // Assert
            this.cosmosRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None), Times.Once);
            Assert.IsTrue(result);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFalse_WhenModelDoesNotExist()
        {
            // Arrange
            this.cosmosRepositoryMock.Setup(repo => repo.GetAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None)).ReturnsAsync((TestModel)null);

            // Act
            var result = await this.repository.DeleteAsync(Guid.NewGuid());

            // Assert
            this.cosmosRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), CancellationToken.None), Times.Never);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task CountAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            this.cosmosRepositoryMock.Setup(repo => repo.CountAsync(CancellationToken.None)).ReturnsAsync(5);

            // Act
            var result = await this.repository.CountAsync();

            // Assert
            Assert.AreEqual(5, result);
        }

        [Test]
        public async Task CountAsync_WithPredicate_ShouldReturnCorrectCount()
        {
            // Arrange
            Expression<Func<TestModel, bool>> predicate = model => model.Id != null;

            this.cosmosRepositoryMock.Setup(repo => repo.CountAsync(It.IsAny<Expression<Func<TestModel, bool>>>(), CancellationToken.None)).ReturnsAsync(3);

            // Act
            var result = await this.repository.CountAsync(predicate);

            // Assert
            Assert.AreEqual(3, result);
        }

        // Additional helper class for testing
        public class TestCosmosDbRepository : BaseCosmosDbRepository<TestModel>
        {
            public TestCosmosDbRepository(IRepository<TestModel> cosmosRepository) : base(cosmosRepository)
            {
            }
        }
        public class TestModel : IItem
        {
            public string Id { get; set; }
            public string Type { get; set; }
            public string PartitionKey { get; }
            public string Name { get; set; }
        }
    }
}

namespace AIC.Core.Services.Networking.Tests.Implementations
{
    using NUnit.Framework;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AIC.Core.Models.Networking.Connections.Contracts;
    using AIC.Core.Models.Networking.Connections.Implementations;
    using AIC.Core.Models.Networking.Contracts;
    using AIC.Core.Services.Networking.Connections.Contracts;
    using AIC.Core.Services.Networking.Connections.Provisioning.Contracts;
    using AIC.Core.Services.Networking.Implementations;
    using Microsoft.Extensions.Logging;

    [TestFixture]
    public class DefaultNetworkServiceTests
    {
        private Mock<ILogger> loggerMock;
        private Mock<IConnectionProvisioningService> connectionProvisioningServiceMock;
        private Mock<IConnectionHandlingService> connectionHandlingServiceMock;
        private DefaultNetworkService service;

        [SetUp]
        public void SetUp()
        {
            this.loggerMock = new Mock<ILogger>();
            this.connectionProvisioningServiceMock = new Mock<IConnectionProvisioningService>();
            this.connectionHandlingServiceMock = new Mock<IConnectionHandlingService>();

            this.service = new DefaultNetworkService(this.loggerMock.Object, this.connectionProvisioningServiceMock.Object);
        }

        [Test]
        public async Task ConnectAsync_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var connectionInfo = new ConnectionInformationType();
            this.connectionProvisioningServiceMock
                .Setup(cps => cps.GetConnectionInstanceAsync(connectionInfo))
                .ThrowsAsync(new Exception("Simulated exception"));

            // Act
            await this.service.ConnectAsync(connectionInfo);

            // Assert
            this.loggerMock.Verify(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task DisconnectAsync_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var connectionId = Guid.NewGuid();
            this.connectionHandlingServiceMock
                .Setup(chs => chs.DisconnectAsync())
                .ThrowsAsync(new Exception("Simulated exception"));

            var connections = this.service.GetType()
                .BaseType
                .GetField("connections", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as IDictionary<Guid, IConnectionHandlingService>;
            connections.Add(connectionId, this.connectionHandlingServiceMock.Object);

            // Act
            await this.service.DisconnectAsync(connectionId);

            // Assert
            this.loggerMock.Verify(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task GetConnectionsAsync_ShouldReturnAllConnections_WhenConnectionTypeIsNull()
        {
            // Arrange
            var connectionId1 = Guid.NewGuid();
            var connectionId2 = Guid.NewGuid();

            var connections = this.service.GetType()
                .BaseType
                .GetField("connections", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as IDictionary<Guid, IConnectionHandlingService>;
            connections.Add(connectionId1, this.connectionHandlingServiceMock.Object);
            connections.Add(connectionId2, this.connectionHandlingServiceMock.Object);

            // Act
            var result = await this.service.GetConnectionsAsync(null);

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetConnectionsAsync_ShouldReturnFilteredConnections_WhenConnectionTypeIsProvided()
        {
            // Arrange
            var connectionInfoType = new ConnectionInformationType();
            this.connectionHandlingServiceMock
                .Setup(chs => chs.ConnectionInformation.Type)
                .Returns(connectionInfoType);

            var connections = this.service.GetType()
                .BaseType
                .GetField("connections", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as IDictionary<Guid, IConnectionHandlingService>;
            connections.Add(Guid.NewGuid(), this.connectionHandlingServiceMock.Object);

            // Act
            var result = await this.service.GetConnectionsAsync(connectionInfoType);

            // Assert
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task GetConnectionAsync_ShouldReturnCorrectConnection()
        {
            // Arrange
            var connectionId = Guid.NewGuid();

            var connections = this.service.GetType()
                .BaseType
                .GetField("connections", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as IDictionary<Guid, IConnectionHandlingService>;
            connections.Add(connectionId, this.connectionHandlingServiceMock.Object);

            // Act
            var result = await this.service.GetConnectionAsync(connectionId);

            // Assert
            Assert.AreEqual(this.connectionHandlingServiceMock.Object, result);
        }

        [Test]
        public void GetConnectionAsync_ShouldThrowKeyNotFoundException_WhenConnectionDoesNotExist()
        {
            // Arrange
            var connectionId = Guid.NewGuid();

            // Act & Assert
            Assert.ThrowsAsync<KeyNotFoundException>(() => this.service.GetConnectionAsync(connectionId));
        }

        [Test]
        public async Task SendCommandAsync_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var commandMock = new Mock<INetworkCommand>();
            this.connectionHandlingServiceMock
                .Setup(chs => chs.SendCommandAsync(It.IsAny<INetworkCommand>()))
                .ThrowsAsync(new Exception("Simulated exception"));

            var connections = this.service.GetType()
                .BaseType
                .GetField("connections", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as IDictionary<Guid, IConnectionHandlingService>;
            connections.Add(Guid.NewGuid(), this.connectionHandlingServiceMock.Object);

            // Act
            await this.service.SendCommandAsync(commandMock.Object);

            // Assert
            this.loggerMock.Verify(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DisconnectInternalAsync_ShouldThrowException_WhenConnectionDoesNotExist()
        {
            // Arrange
            var connectionId = Guid.NewGuid();

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => this.service.DisconnectAsync(connectionId));
        }

        [Test]
        public async Task SendCommandInternalAsync_ShouldSendCommandToAllConnections()
        {
            // Arrange
            var commandMock = new Mock<INetworkCommand>();

            var connections = this.service.GetType()
                .BaseType
                .GetField("connections", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as IDictionary<Guid, IConnectionHandlingService>;
            connections.Add(Guid.NewGuid(), this.connectionHandlingServiceMock.Object);

            // Act
            await this.service.SendCommandAsync(commandMock.Object);

            // Assert
            this.connectionHandlingServiceMock.Verify(chs => chs.SendCommandAsync(commandMock.Object), Times.Once);
        }

        [Test]
        public void SendCommandInternalAsync_ShouldThrowException_WhenNoConnectionsExist()
        {
            // Arrange
            var commandMock = new Mock<INetworkCommand>();

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => this.service.SendCommandAsync(commandMock.Object));
        }

        [Test]
        public async Task OnDataReceived_ShouldInvokeDataReceivedEvent()
        {
            // Arrange
            var data = new byte[] { 0x01, 0x02, 0x03 };
            bool eventInvoked = false;

            this.service.DataReceived += (receivedData) =>
            {
                eventInvoked = true;
                return Task.CompletedTask;
            };

            // Act
            var onDataReceivedMethod = this.service.GetType()
                .BaseType
                .GetMethod("OnDataReceived", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            await (Task)onDataReceivedMethod.Invoke(this.service, new object[] { data });

            // Assert
            Assert.IsTrue(eventInvoked);
        }
    }


}
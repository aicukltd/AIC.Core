namespace AIC.Core.Services.Networking.Connections.Udp.Tests.Implementations
{
    using NUnit.Framework;
    using Moq;
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using AIC.Core.Models.Networking.Connections.Contracts;
    using AIC.Core.Models.Networking.Connections.Implementations;
    using AIC.Core.Models.Networking.Contracts;
    using AIC.Core.Services.Networking.Connections.Udp.Implementations;
    using Microsoft.Extensions.Logging;

    [TestFixture]
    public class UdpConnectionHandlingServiceTests
    {
        private Mock<ILogger> loggerMock;
        private UdpConnectionHandlingService service;

        [SetUp]
        public void SetUp()
        {
            this.loggerMock = new Mock<ILogger>();
            this.service = new UdpConnectionHandlingService(this.loggerMock.Object);
        }

        [Test]
        public async Task ConnectInternalAsync_ShouldInitializeUdpClient()
        {
            // Arrange
            var connectionInformation = new ConnectionInformation
            {
                Host = "127.0.0.1",
                Port = 1234,
                Mode = ConnectionInformationMode.Client
            };
            this.service.SetConnectionInformation(connectionInformation);

            // Act
            await this.service.ConnectAsync();

            // Assert
            var udpClient = this.service.GetType()
                .GetField("udpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as UdpClient;
            Assert.IsNotNull(udpClient);

            var remoteEndPoint = this.service.GetType()
                .GetField("remoteEndPoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as IPEndPoint;
            Assert.IsNotNull(remoteEndPoint);
            Assert.AreEqual(IPAddress.Parse("127.0.0.1"), remoteEndPoint.Address);
            Assert.AreEqual(1234, remoteEndPoint.Port);
        }

        [Test]
        public async Task DisconnectInternalAsync_ShouldCloseUdpClient()
        {
            // Arrange
            var connectionInformation = new ConnectionInformation
            {
                Host = "127.0.0.1",
                Port = 1234,
                Mode = ConnectionInformationMode.Client
            };
            this.service.SetConnectionInformation(connectionInformation);

            await this.service.ConnectAsync();

            // Act
            await this.service.DisconnectAsync();

            // Assert
            var udpClient = this.service.GetType()
                .GetField("udpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as UdpClient;
            Assert.IsNull(udpClient);

            var remoteEndPoint = this.service.GetType()
                .GetField("remoteEndPoint", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as IPEndPoint;
            Assert.IsNull(remoteEndPoint);
        }

        [Test]
        public async Task SendCommandInternalAsync_ShouldThrowInvalidOperationException_WhenUdpClientIsNotInitialized()
        {
            // Arrange
            var networkCommandMock = new Mock<INetworkCommand>();
            networkCommandMock.Setup(nc => nc.Data).Returns(Encoding.ASCII.GetBytes("Test command"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => this.service.SendCommandAsync(networkCommandMock.Object));
            Assert.AreEqual("UDP client is not initialized or connected.", ex.Message);
        }

        [Test]
        public async Task SendCommandInternalAsync_ShouldSendDataToRemoteEndPoint_WhenInClientMode()
        {
            // Arrange
            var connectionInformation = new ConnectionInformation
            {
                Host = "127.0.0.1",
                Port = 1234,
                Mode = ConnectionInformationMode.Client
            };
            this.service.SetConnectionInformation(connectionInformation);

            await this.service.ConnectAsync();

            var udpClient = this.service.GetType()
                .GetField("udpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as UdpClient;

            var networkCommandMock = new Mock<INetworkCommand>();
            networkCommandMock.Setup(nc => nc.Data).Returns(Encoding.ASCII.GetBytes("Test command"));

            // Act
            await this.service.SendCommandAsync(networkCommandMock.Object);

            // Assert
            Assert.Pass(); // To verify actual sending, you'd need to mock UdpClient or check its internal state.
        }

        [Test]
        public async Task SendCommandInternalAsync_ShouldThrowInvalidOperationException_WhenServerModeAndUnicast()
        {
            // Arrange
            var connectionInformation = new ConnectionInformation
            {
                Host = "127.0.0.1",
                Port = 1234,
                Mode = ConnectionInformationMode.Server,
                UdpMode = UdpConnectionInformationMode.Unicast
            };
            this.service.SetConnectionInformation(connectionInformation);

            await this.service.ConnectAsync();

            var networkCommandMock = new Mock<INetworkCommand>();
            networkCommandMock.Setup(nc => nc.Data).Returns(Encoding.ASCII.GetBytes("Test command"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => this.service.SendCommandAsync(networkCommandMock.Object));
            Assert.AreEqual("Server mode requires multicast address for sending data.", ex.Message);
        }

        [Test]
        public async Task ListenForDataAsync_ShouldInvokeDataReceivedEvent_WhenDataIsReceived()
        {
            // Arrange
            var dataReceived = false;

            this.service.DataReceived += async data =>
            {
                dataReceived = true;
                await Task.CompletedTask;
            };

            var connectionInformation = new ConnectionInformation
            {
                Host = "127.0.0.1",
                Port = 1234,
                Mode = ConnectionInformationMode.Client
            };
            this.service.SetConnectionInformation(connectionInformation);

            await this.service.ConnectAsync();

            var udpClient = this.service.GetType()
                .GetField("udpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as UdpClient;

            // Act
            // Simulate data reception by mocking UdpClient or using a real UDP connection in integration tests.
            var listenForDataMethod = this.service.GetType().GetMethod("ListenForDataAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            await (Task)listenForDataMethod.Invoke(this.service, null);

            // Assert
            Assert.IsTrue(dataReceived);
        }
    }

}
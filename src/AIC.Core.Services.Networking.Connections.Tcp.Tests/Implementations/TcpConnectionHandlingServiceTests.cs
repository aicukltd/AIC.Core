namespace AIC.Core.Services.Networking.Connections.Tcp.Tests.Implementations
{
    using NUnit.Framework;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using AIC.Core.Models.Networking.Connections.Contracts;
    using AIC.Core.Models.Networking.Connections.Implementations;
    using AIC.Core.Models.Networking.Contracts;
    using AIC.Core.Services.Networking.Connections.Tcp.Implementations;
    using System.Timers;
    using Microsoft.Extensions.Logging;

    [TestFixture]
    public class TcpConnectionHandlingServiceTests
    {
        private Mock<ILogger> loggerMock;
        private TcpConnectionHandlingService service;

        [SetUp]
        public void SetUp()
        {
            this.loggerMock = new Mock<ILogger>();
            this.service = new TcpConnectionHandlingService(this.loggerMock.Object);
        }

        [Test]
        public async Task ConnectInternalAsync_ShouldSetupAsClient_WhenModeIsClient()
        {
            // Arrange
            var connectionInformation = new ConnectionInformation
            {
                Host = "127.0.0.1",
                Port = 1234,
                Mode = ConnectionInformationMode.Client
            };
            this.service.SetConnectionInformation(connectionInformation);

            var tcpClientMock = new Mock<TcpClient>();
            var networkStreamMock = new Mock<NetworkStream>(new MemoryStream());

            tcpClientMock.Setup(c => c.ConnectAsync(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            tcpClientMock.Setup(c => c.GetStream()).Returns(networkStreamMock.Object);

            var tcpClientField = this.service.GetType().GetField("tcpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            tcpClientField.SetValue(this.service, tcpClientMock.Object);

            // Act
            await this.service.ConnectAsync();

            // Assert
            tcpClientMock.Verify(c => c.ConnectAsync("127.0.0.1", 1234), Times.Once);
            networkStreamMock.Verify(ns => ns.ReadAsync(It.IsAny<byte[]>(), 0, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task ConnectInternalAsync_ShouldSetupAsServer_WhenModeIsServer()
        {
            // Arrange
            var connectionInformation = new ConnectionInformation
            {
                Host = "127.0.0.1",
                Port = 1234,
                Mode = ConnectionInformationMode.Client
            };
            this.service.SetConnectionInformation(connectionInformation);

            var tcpListenerMock = new Mock<TcpListener>(IPAddress.Parse("127.0.0.1"), 1234);
            tcpListenerMock.Setup(tl => tl.Start());

            var tcpListenerField = this.service.GetType().GetField("tcpListener", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            tcpListenerField.SetValue(this.service, tcpListenerMock.Object);

            // Act
            await this.service.ConnectAsync();

            // Assert
            tcpListenerMock.Verify(tl => tl.Start(), Times.Once);
            Assert.IsTrue(tcpListenerMock.Object.Server.IsBound);
        }

        [Test]
        public async Task DisconnectInternalAsync_ShouldCloseAllConnections()
        {
            // Arrange
            var tcpClientMock = new Mock<TcpClient>();
            var networkStreamMock = new Mock<NetworkStream>(new MemoryStream());

            tcpClientMock.Setup(c => c.GetStream()).Returns(networkStreamMock.Object);
            tcpClientMock.Setup(c => c.Close());
            networkStreamMock.Setup(ns => ns.Close());
            networkStreamMock.Setup(ns => ns.DisposeAsync()).Returns(ValueTask.CompletedTask);

            var tcpClientsField = this.service.GetType().GetField("tcpClients", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tcpClients = (IDictionary<Guid, (TcpClient, NetworkStream)>)tcpClientsField.GetValue(this.service);
            tcpClients.Add(Guid.NewGuid(), (tcpClientMock.Object, networkStreamMock.Object));

            // Act
            await this.service.DisconnectAsync();

            // Assert
            tcpClientMock.Verify(c => c.Close(), Times.Once);
            networkStreamMock.Verify(ns => ns.Close(), Times.Once);
            networkStreamMock.Verify(ns => ns.DisposeAsync(), Times.Once);
            Assert.IsEmpty(tcpClients);
        }

        [Test]
        public async Task SendCommandInternalAsync_ShouldSendCommandToServer_WhenModeIsClient()
        {
            // Arrange
            var connectionInformation = new ConnectionInformation
            {
                Host = "127.0.0.1",
                Port = 1234,
                Mode = ConnectionInformationMode.Client
            };
            this.service.SetConnectionInformation(connectionInformation);

            var networkStreamMock = new Mock<NetworkStream>(new MemoryStream());
            networkStreamMock.Setup(ns => ns.WriteAsync(It.IsAny<byte[]>(), 0, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            networkStreamMock.Setup(ns => ns.FlushAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var tcpClientField = this.service.GetType().GetField("tcpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            tcpClientField.SetValue(this.service, new TcpClient());

            var networkStreamField = this.service.GetType().GetField("networkStream", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            networkStreamField.SetValue(this.service, networkStreamMock.Object);

            var networkCommandMock = new Mock<INetworkCommand>();
            networkCommandMock.Setup(nc => nc.Data).Returns(Encoding.ASCII.GetBytes("Test command"));

            // Act
            await this.service.SendCommandAsync(networkCommandMock.Object);

            // Assert
            networkStreamMock.Verify(ns => ns.WriteAsync(It.IsAny<byte[]>(), 0, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            networkStreamMock.Verify(ns => ns.FlushAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task SendCommandInternalAsync_ShouldSendCommandToClients_WhenModeIsServer()
        {
            // Arrange
            var connectionInformation = new ConnectionInformation
            {
                Host = "127.0.0.1",
                Port = 1234,
                Mode = ConnectionInformationMode.Client
            };
            this.service.SetConnectionInformation(connectionInformation);

            var tcpClientMock = new Mock<TcpClient>();
            var networkStreamMock = new Mock<NetworkStream>(new MemoryStream());

            tcpClientMock.Setup(c => c.GetStream()).Returns(networkStreamMock.Object);
            networkStreamMock.Setup(ns => ns.WriteAsync(It.IsAny<byte[]>(), 0, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            networkStreamMock.Setup(ns => ns.FlushAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var tcpClientsField = this.service.GetType().GetField("tcpClients", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var tcpClients = (IDictionary<Guid, (TcpClient, NetworkStream)>)tcpClientsField.GetValue(this.service);
            tcpClients.Add(Guid.NewGuid(), (tcpClientMock.Object, networkStreamMock.Object));

            var networkCommandMock = new Mock<INetworkCommand>();
            networkCommandMock.Setup(nc => nc.Data).Returns(Encoding.ASCII.GetBytes("Test command"));

            // Act
            await this.service.SendCommandAsync(networkCommandMock.Object);

            // Assert
            networkStreamMock.Verify(ns => ns.WriteAsync(It.IsAny<byte[]>(), 0, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            networkStreamMock.Verify(ns => ns.FlushAsync(It.IsAny<CancellationToken>()), Times.Once);
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

            var networkStreamMock = new Mock<NetworkStream>(new MemoryStream());
            networkStreamMock.Setup(ns => ns.ReadAsync(It.IsAny<byte[]>(), 0, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Encoding.ASCII.GetBytes("Test data").Length)
                .Callback<byte[], int, int, CancellationToken>((buffer, offset, count, token) =>
                {
                    Array.Copy(Encoding.ASCII.GetBytes("Test data"), buffer, Encoding.ASCII.GetBytes("Test data").Length);
                });

            var networkStreamField = this.service.GetType().GetField("networkStream", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            networkStreamField.SetValue(this.service, networkStreamMock.Object);

            // Act
            var listenForDataMethod = this.service.GetType().GetMethod("ListenForDataAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            await (Task)listenForDataMethod.Invoke(this.service, null);

            // Assert
            Assert.IsTrue(dataReceived);
        }

        [Test]
        public void ConnectInternalAsync_ShouldThrowArgumentOutOfRangeException_WhenInvalidModeIsProvided()
        {
            // Arrange
            var connectionInformation = new ConnectionInformation
            {
                Host = "127.0.0.1",
                Port = 1234,
                Mode = (ConnectionInformationMode)999
            };
            this.service.SetConnectionInformation(connectionInformation);

            // Act & Assert
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => this.service.ConnectAsync());
        }
    }

}
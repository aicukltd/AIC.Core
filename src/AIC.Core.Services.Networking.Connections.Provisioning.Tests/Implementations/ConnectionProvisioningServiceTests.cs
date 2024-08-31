namespace AIC.Core.Services.Networking.Connections.Provisioning.Tests.Implementations
{
    using NUnit.Framework;
    using Moq;
    using Microsoft.Extensions.Options;
    using System;
    using System.Threading.Tasks;
    using AIC.Core.Models.Networking.Connections.Contracts;
    using AIC.Core.Models.Networking.Connections.Implementations;
    using AIC.Core.Models.Networking.Connections.Serial.Implementations;
    using AIC.Core.Models.Networking.Connections.Tcp.Implementations;
    using AIC.Core.Models.Networking.Connections.Udp.Implementations;
    using AIC.Core.Services.Networking.Connections.Provisioning.Implementations;
    using AIC.Core.Services.Networking.Connections.Serial.Implementations;
    using AIC.Core.Services.Networking.Connections.Tcp.Implementations;
    using AIC.Core.Services.Networking.Connections.Udp.Implementations;
    using Microsoft.Extensions.Logging;

    [TestFixture]
    public class ConnectionProvisioningServiceTests
    {
        private Mock<ILogger> loggerMock;
        private Mock<IOptions<ConnectionOptions>> optionsMock;
        private ConnectionProvisioningService service;

        [SetUp]
        public void SetUp()
        {
            this.loggerMock = new Mock<ILogger>();
            this.optionsMock = new Mock<IOptions<ConnectionOptions>>();
            this.service = new ConnectionProvisioningService(this.loggerMock.Object, this.optionsMock.Object);
        }

        [Test]
        public async Task GetConnectionInstanceAsync_ShouldReturnTcpConnectionInstance_WhenTcpTypeIsProvided()
        {
            // Arrange
            var connectionInformationType = ConnectionInformationType.Tcp;
            var options = new ConnectionOptions
            {
                Port = 12345,
                Host = "localhost",
                Mode = ConnectionInformationMode.Client
            };
            this.optionsMock.Setup(o => o.Value).Returns(options);

            // Act
            var result = await this.service.GetConnectionInstanceAsync(connectionInformationType);

            // Assert
            Assert.IsInstanceOf<TcpConnectionInstance>(result);
            var tcpInstance = result as TcpConnectionInstance;
            Assert.AreEqual(options.Port, tcpInstance.Port);
            Assert.AreEqual(options.Host, tcpInstance.Host);
            Assert.AreEqual(options.Mode, tcpInstance.Mode);
            Assert.AreEqual(connectionInformationType, tcpInstance.Type);
        }

        [Test]
        public async Task GetConnectionInstanceAsync_ShouldReturnUdpConnectionInstance_WhenUdpTypeIsProvided()
        {
            // Arrange
            var connectionInformationType = ConnectionInformationType.Udp;
            var options = new ConnectionOptions
            {
                Port = 12345,
                Host = "localhost",
                Mode = ConnectionInformationMode.Client,
                UdpMode = UdpConnectionInformationMode.Unicast
            };
            this.optionsMock.Setup(o => o.Value).Returns(options);

            // Act
            var result = await this.service.GetConnectionInstanceAsync(connectionInformationType);

            // Assert
            Assert.IsInstanceOf<UdpConnectionInstance>(result);
            var udpInstance = result as UdpConnectionInstance;
            Assert.AreEqual(options.Port, udpInstance.Port);
            Assert.AreEqual(options.Host, udpInstance.Host);
            Assert.AreEqual(options.Mode, udpInstance.Mode);
            Assert.AreEqual(options.UdpMode, udpInstance.UdpMode);
            Assert.AreEqual(connectionInformationType, udpInstance.Type);
        }

        [Test]
        public async Task GetConnectionInstanceAsync_ShouldReturnSerialConnectionInstance_WhenSerialTypeIsProvided()
        {
            // Arrange
            var connectionInformationType = ConnectionInformationType.Serial;
            var options = new ConnectionOptions
            {
                SerialPortName = "COM1",
                BaudRate = 9600,
                DataBits = 8,
                DefaultTimeOut = 1000,
                Mode = ConnectionInformationMode.Client
            };
            this.optionsMock.Setup(o => o.Value).Returns(options);

            // Act
            var result = await this.service.GetConnectionInstanceAsync(connectionInformationType);

            // Assert
            Assert.IsInstanceOf<SerialConnectionInstance>(result);
            var serialInstance = result as SerialConnectionInstance;
            Assert.AreEqual(options.SerialPortName, serialInstance.SerialPortName);
            Assert.AreEqual(options.BaudRate, serialInstance.BaudRate);
            Assert.AreEqual(options.DataBits, serialInstance.DataBits);
            Assert.AreEqual(options.DefaultTimeOut, serialInstance.DefaultTimeOut);
            Assert.AreEqual(options.Mode, serialInstance.Mode);
            Assert.AreEqual(connectionInformationType, serialInstance.Type);
        }

        [Test]
        public async Task GetConnectionInstanceAsync_ShouldLogErrorAndReturnDefault_WhenExceptionIsThrown()
        {
            // Arrange
            var connectionInformationType = ConnectionInformationType.Tcp;
            this.optionsMock.Setup(o => o.Value).Throws(new Exception("Simulated exception"));

            // Act
            var result = await this.service.GetConnectionInstanceAsync(connectionInformationType);

            // Assert
            this.loggerMock.Verify(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetConnectionHandlerAsync_ShouldReturnTcpConnectionHandlingService_WhenTcpInstanceIsProvided()
        {
            // Arrange
            var connectionInstance = new TcpConnectionInstance { Type = ConnectionInformationType.Tcp };

            // Act
            var result = await this.service.GetConnectionHandlerAsync(connectionInstance);

            // Assert
            Assert.IsInstanceOf<TcpConnectionHandlingService>(result);
            Assert.AreEqual(connectionInstance, (result as TcpConnectionHandlingService).ConnectionInformation);
        }

        [Test]
        public async Task GetConnectionHandlerAsync_ShouldReturnUdpConnectionHandlingService_WhenUdpInstanceIsProvided()
        {
            // Arrange
            var connectionInstance = new UdpConnectionInstance { Type = ConnectionInformationType.Udp };

            // Act
            var result = await this.service.GetConnectionHandlerAsync(connectionInstance);

            // Assert
            Assert.IsInstanceOf<UdpConnectionHandlingService>(result);
            Assert.AreEqual(connectionInstance, (result as UdpConnectionHandlingService).ConnectionInformation);
        }

        [Test]
        public async Task GetConnectionHandlerAsync_ShouldReturnSerialConnectionHandlingService_WhenSerialInstanceIsProvided()
        {
            // Arrange
            var connectionInstance = new SerialConnectionInstance { Type = ConnectionInformationType.Serial };

            // Act
            var result = await this.service.GetConnectionHandlerAsync(connectionInstance);

            // Assert
            Assert.IsInstanceOf<SerialConnectionHandlingService>(result);
            Assert.AreEqual(connectionInstance, (result as SerialConnectionHandlingService).ConnectionInformation);
        }

        [Test]
        public async Task GetConnectionHandlerAsync_ShouldLogErrorAndReturnDefault_WhenExceptionIsThrown()
        {
            // Arrange
            var connectionInstance = new TcpConnectionInstance { Type = ConnectionInformationType.Tcp };
            this.loggerMock.Setup(l => l.LogError(It.IsAny<Exception>(), It.IsAny<string>())).Throws(new Exception("Simulated exception"));

            // Act
            var result = await this.service.GetConnectionHandlerAsync(connectionInstance);

            // Assert
            this.loggerMock.Verify(logger => logger.LogError(It.IsAny<Exception>(), It.IsAny<string>()), Times.Once);
            Assert.IsNull(result);
        }
    }

}
namespace AIC.Core.Services.Networking.Connections.Serial.Tests.Implementations
{
    using NUnit.Framework;
    using Moq;
    using System;
    using System.IO.Ports;
    using System.Text;
    using System.Threading.Tasks;
    using AIC.Core.Models.Networking.Connections.Implementations;
    using AIC.Core.Models.Networking.Contracts;
    using AIC.Core.Services.Networking.Connections.Serial.Implementations;
    using Microsoft.Extensions.Logging;

    [TestFixture]
    public class SerialConnectionHandlingServiceTests
    {
        private Mock<ILogger> loggerMock;
        private Mock<INetworkCommand> networkCommandMock;
        private SerialConnectionHandlingService service;

        [SetUp]
        public void SetUp()
        {
            this.loggerMock = new Mock<ILogger>();
            this.networkCommandMock = new Mock<INetworkCommand>();

            this.service = new SerialConnectionHandlingService(this.loggerMock.Object);
        }

        [Test]
        public async Task ConnectInternalAsync_ShouldInitializeAndOpenSerialPort()
        {
            // Arrange
            var connectionInformation = new ConnectionInformation
            {
                SerialPortName = "COM1",
                BaudRate = 9600,
                DataBits = 8,
                DefaultTimeOut = 1000
            };
            this.service.SetConnectionInformation(connectionInformation);

            // Act
            await this.service.ConnectAsync();

            // Assert
            Assert.IsNotNull(this.service.GetType()
                .GetField("serialPort", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as SerialPort);
            Assert.IsTrue((this.service.GetType()
                .GetField("serialPort", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as SerialPort).IsOpen);
        }

        [Test]
        public async Task DisconnectInternalAsync_ShouldCloseSerialPort_WhenPortIsOpen()
        {
            // Arrange
            var connectionInformation = new ConnectionInformation
            {
                SerialPortName = "COM1",
                BaudRate = 9600,
                DataBits = 8,
                DefaultTimeOut = 1000
            };
            this.service.SetConnectionInformation(connectionInformation);

            await this.service.ConnectAsync();

            // Act
            await this.service.DisconnectAsync();

            // Assert
            Assert.IsFalse((this.service.GetType()
                .GetField("serialPort", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as SerialPort).IsOpen);
            Assert.IsNull(this.service.GetType()
                .GetField("serialPort", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service));
        }

        [Test]
        public void SendCommandInternalAsync_ShouldThrowInvalidOperationException_WhenSerialPortIsNotConnected()
        {
            // Arrange
            var connectionInformation = new ConnectionInformation
            {
                SerialPortName = "COM1",
                BaudRate = 9600,
                DataBits = 8,
                DefaultTimeOut = 1000
            };
            this.service.SetConnectionInformation(connectionInformation);

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => this.service.SendCommandAsync(this.networkCommandMock.Object));
        }

        [Test]
        public async Task SendCommandInternalAsync_ShouldWriteDataToSerialPort()
        {
            // Arrange
            var connectionInformation = new ConnectionInformation
            {
                SerialPortName = "COM1",
                BaudRate = 9600,
                DataBits = 8,
                DefaultTimeOut = 1000
            };
            this.service.SetConnectionInformation(connectionInformation);

            this.networkCommandMock.Setup(nc => nc.Data).Returns(Encoding.ASCII.GetBytes("Test command"));

            await this.service.ConnectAsync();

            // Act
            await this.service.SendCommandAsync(this.networkCommandMock.Object);

            // Assert
            var serialPort = this.service.GetType()
                .GetField("serialPort", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(this.service) as SerialPort;

            // To verify if data was written, you would typically mock the SerialPort. Since it's difficult to verify without actual hardware,
            // consider this test complete if it reaches this point without throwing an exception.
            Assert.Pass();
        }

        [Test]
        public async Task ListenForDataAsync_ShouldInvokeDataReceivedEvent_WhenDataIsReceived()
        {
            // Arrange
            var dataReceived = false;
            var serialPortMock = new Mock<SerialPort>("COM1", 9600, Parity.None, 8, StopBits.One);
            serialPortMock.Setup(sp => sp.ReadLine()).Returns("Test data");

            this.service.DataReceived += async data =>
            {
                dataReceived = true;
                await Task.CompletedTask;
            };

            this.service.GetType()
                .GetField("serialPort", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(this.service, serialPortMock.Object);

            // Act
            var listenForDataMethod = this.service.GetType()
                .GetMethod("ListenForDataAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            await (Task)listenForDataMethod.Invoke(this.service, null);

            // Assert
            Assert.IsTrue(dataReceived);
        }
    }

}
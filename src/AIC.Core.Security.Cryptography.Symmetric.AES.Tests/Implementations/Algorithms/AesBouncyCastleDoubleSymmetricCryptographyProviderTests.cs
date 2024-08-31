namespace AIC.Core.Security.Cryptography.Symmetric.AES.Tests.Implementations.Algorithms;

    using NUnit.Framework;
    using Moq;
    using System;
    using System.Text;
    using System.Security.Cryptography;

  
        using AIC.Core.Security.Cryptography.Symmetric.AES.Implementations.Algorithms;
        using AIC.Core.Security.Cryptography.Symmetric.Contracts;

        [TestFixture]
        public class AesBouncyCastleDoubleSymmetricCryptographyProviderTests
        {
            private Mock<ISymmetricCryptographyProvider> mockPrimaryProvider;
            private Mock<ISymmetricCryptographyProvider> mockSecondaryProvider;
            private AesBouncyCastleDoubleSymmetricCryptographyProvider doubleProvider;

            [SetUp]
            public void SetUp()
            {
                this.mockPrimaryProvider = new Mock<ISymmetricCryptographyProvider>();
                this.mockSecondaryProvider = new Mock<ISymmetricCryptographyProvider>();
                this.doubleProvider = new AesBouncyCastleDoubleSymmetricCryptographyProvider(
                    this.mockPrimaryProvider.Object,
                    this.mockSecondaryProvider.Object);
            }

            #region Byte Array Encryption and Decryption Tests

            [Test]
            public void Encrypt_ByteArray_WithValidKeysAndIVs_ShouldEncryptDataTwice()
            {
                // Arrange
                var data = Encoding.UTF8.GetBytes("Test Data");
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };
                var primaryIv = new byte[] { 7, 8, 9 };
                var secondaryIv = new byte[] { 10, 11, 12 };

                var primaryEncrypted = new byte[] { 13, 14, 15 };
                var secondaryEncrypted = new byte[] { 16, 17, 18 };

                this.mockPrimaryProvider
                    .Setup(p => p.Encrypt(data, primaryKey, primaryIv))
                    .Returns(primaryEncrypted)
                    .Verifiable();

                this.mockSecondaryProvider
                    .Setup(p => p.Encrypt(primaryEncrypted, secondaryKey, secondaryIv))
                    .Returns(secondaryEncrypted)
                    .Verifiable();

                // Act
                var result = this.doubleProvider.Encrypt(data, primaryKey, secondaryKey, primaryIv, secondaryIv);

                // Assert
                Assert.AreEqual(secondaryEncrypted, result);

                this.mockPrimaryProvider.Verify(p => p.Encrypt(data, primaryKey, primaryIv), Times.Once);
                this.mockSecondaryProvider.Verify(p => p.Encrypt(primaryEncrypted, secondaryKey, secondaryIv), Times.Once);
            }

            [Test]
            public void Decrypt_ByteArray_WithValidKeysAndIVs_ShouldDecryptDataTwice()
            {
                // Arrange
                var encryptedData = new byte[] { 16, 17, 18 };
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };
                var primaryIv = new byte[] { 7, 8, 9 };
                var secondaryIv = new byte[] { 10, 11, 12 };

                var primaryDecrypted = new byte[] { 13, 14, 15 };
                var finalDecrypted = Encoding.UTF8.GetBytes("Test Data");

                this.mockPrimaryProvider
                    .Setup(p => p.Decrypt(encryptedData, primaryKey, primaryIv))
                    .Returns(primaryDecrypted)
                    .Verifiable();

                this.mockSecondaryProvider
                    .Setup(p => p.Decrypt(primaryDecrypted, secondaryKey, secondaryIv))
                    .Returns(finalDecrypted)
                    .Verifiable();

                // Act
                var result = this.doubleProvider.Decrypt(encryptedData, primaryKey, secondaryKey, primaryIv, secondaryIv);

                // Assert
                Assert.AreEqual(finalDecrypted, result);

                this.mockPrimaryProvider.Verify(p => p.Decrypt(encryptedData, primaryKey, primaryIv), Times.Once);
                this.mockSecondaryProvider.Verify(p => p.Decrypt(primaryDecrypted, secondaryKey, secondaryIv), Times.Once);
            }

            [Test]
            public void Encrypt_ByteArray_WithNullData_ShouldThrowArgumentNullException()
            {
                // Arrange
                byte[] data = null;
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };
                var primaryIv = new byte[] { 7, 8, 9 };
                var secondaryIv = new byte[] { 10, 11, 12 };

                // Act & Assert
                Assert.Throws<ArgumentNullException>(() =>
                    this.doubleProvider.Encrypt(data, primaryKey, secondaryKey, primaryIv, secondaryIv));

                // Ensure no encryption methods were called
                this.mockPrimaryProvider.Verify(p => p.Encrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()),
                    Times.Never);
                this.mockSecondaryProvider.Verify(p => p.Encrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()),
                    Times.Never);
            }

            [Test]
            public void Decrypt_ByteArray_WithNullData_ShouldThrowArgumentNullException()
            {
                // Arrange
                byte[] data = null;
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };
                var primaryIv = new byte[] { 7, 8, 9 };
                var secondaryIv = new byte[] { 10, 11, 12 };

                // Act & Assert
                Assert.Throws<ArgumentNullException>(() =>
                    this.doubleProvider.Decrypt(data, primaryKey, secondaryKey, primaryIv, secondaryIv));

                // Ensure no decryption methods were called
                this.mockPrimaryProvider.Verify(p => p.Decrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()),
                    Times.Never);
                this.mockSecondaryProvider.Verify(p => p.Decrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()),
                    Times.Never);
            }

            [Test]
            public void Encrypt_ByteArray_WithInvalidKeysOrIVs_ShouldPropagateExceptions()
            {
                // Arrange
                var data = Encoding.UTF8.GetBytes("Test Data");
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };
                var primaryIv = new byte[] { 7, 8, 9 };
                var secondaryIv = new byte[] { 10, 11, 12 };

                this.mockPrimaryProvider
                    .Setup(p => p.Encrypt(data, primaryKey, primaryIv))
                    .Throws(new CryptographicException("Primary encryption failed"));

                // Act & Assert
                var ex = Assert.Throws<CryptographicException>(() =>
                    this.doubleProvider.Encrypt(data, primaryKey, secondaryKey, primaryIv, secondaryIv));

                Assert.AreEqual("Primary encryption failed", ex.Message);

                this.mockPrimaryProvider.Verify(p => p.Encrypt(data, primaryKey, primaryIv), Times.Once);
                this.mockSecondaryProvider.Verify(p => p.Encrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()),
                    Times.Never);
            }

            [Test]
            public void Decrypt_ByteArray_WithInvalidKeysOrIVs_ShouldPropagateExceptions()
            {
                // Arrange
                var encryptedData = new byte[] { 16, 17, 18 };
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };
                var primaryIv = new byte[] { 7, 8, 9 };
                var secondaryIv = new byte[] { 10, 11, 12 };

                this.mockPrimaryProvider
                    .Setup(p => p.Decrypt(encryptedData, primaryKey, primaryIv))
                    .Throws(new CryptographicException("Primary decryption failed"));

                // Act & Assert
                var ex = Assert.Throws<CryptographicException>(() =>
                    this.doubleProvider.Decrypt(encryptedData, primaryKey, secondaryKey, primaryIv, secondaryIv));

                Assert.AreEqual("Primary decryption failed", ex.Message);

                this.mockPrimaryProvider.Verify(p => p.Decrypt(encryptedData, primaryKey, primaryIv), Times.Once);
                this.mockSecondaryProvider.Verify(p => p.Decrypt(It.IsAny<byte[]>(), It.IsAny<byte[]>(), It.IsAny<byte[]>()),
                    Times.Never);
            }

            #endregion

            #region String Encryption and Decryption Tests

            [Test]
            public void Encrypt_String_WithValidKeys_ShouldEncryptDataTwice()
            {
                // Arrange
                var data = "Test String Data";
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };

                var primaryEncrypted = "PrimaryEncryptedData";
                var secondaryEncrypted = "SecondaryEncryptedData";

                this.mockPrimaryProvider
                    .Setup(p => p.Encrypt(data, primaryKey))
                    .Returns(primaryEncrypted)
                    .Verifiable();

                this.mockSecondaryProvider
                    .Setup(p => p.Encrypt(primaryEncrypted, secondaryKey))
                    .Returns(secondaryEncrypted)
                    .Verifiable();

                // Act
                var result = this.doubleProvider.Encrypt(data, primaryKey, secondaryKey);

                // Assert
                Assert.AreEqual(secondaryEncrypted, result);

                this.mockPrimaryProvider.Verify(p => p.Encrypt(data, primaryKey), Times.Once);
                this.mockSecondaryProvider.Verify(p => p.Encrypt(primaryEncrypted, secondaryKey), Times.Once);
            }

            [Test]
            public void Decrypt_String_WithValidKeys_ShouldDecryptDataTwice()
            {
                // Arrange
                var encryptedData = "SecondaryEncryptedData";
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };

                var primaryDecrypted = "PrimaryDecryptedData";
                var finalDecrypted = "Test String Data";

                this.mockPrimaryProvider
                    .Setup(p => p.Decrypt(encryptedData, primaryKey))
                    .Returns(primaryDecrypted)
                    .Verifiable();

                this.mockSecondaryProvider
                    .Setup(p => p.Decrypt(primaryDecrypted, secondaryKey))
                    .Returns(finalDecrypted)
                    .Verifiable();

                // Act
                var result = this.doubleProvider.Decrypt(encryptedData, primaryKey, secondaryKey);

                // Assert
                Assert.AreEqual(finalDecrypted, result);

                this.mockPrimaryProvider.Verify(p => p.Decrypt(encryptedData, primaryKey), Times.Once);
                this.mockSecondaryProvider.Verify(p => p.Decrypt(primaryDecrypted, secondaryKey), Times.Once);
            }

            [Test]
            public void Encrypt_String_WithNullData_ShouldThrowArgumentNullException()
            {
                // Arrange
                string data = null;
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };

                // Act & Assert
                Assert.ThrowsAsync<ArgumentNullException>(async () =>
                    await Task.Run(() => this.doubleProvider.Encrypt(data, primaryKey, secondaryKey)));

                // Ensure no encryption methods were called
                this.mockPrimaryProvider.Verify(p => p.Encrypt(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
                this.mockSecondaryProvider.Verify(p => p.Encrypt(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
            }

            [Test]
            public void Decrypt_String_WithNullData_ShouldThrowArgumentNullException()
            {
                // Arrange
                string data = null;
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };

                // Act & Assert
                Assert.ThrowsAsync<ArgumentNullException>(async () =>
                    await Task.Run(() => this.doubleProvider.Decrypt(data, primaryKey, secondaryKey)));

                // Ensure no decryption methods were called
                this.mockPrimaryProvider.Verify(p => p.Decrypt(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
                this.mockSecondaryProvider.Verify(p => p.Decrypt(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
            }

            [Test]
            public void Encrypt_String_WithInvalidKeys_ShouldPropagateExceptions()
            {
                // Arrange
                var data = "Test String Data";
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };

                this.mockPrimaryProvider
                    .Setup(p => p.Encrypt(data, primaryKey))
                    .Throws(new CryptographicException("Primary encryption failed"));

                // Act & Assert
                var ex = Assert.Throws<CryptographicException>(() =>
                    this.doubleProvider.Encrypt(data, primaryKey, secondaryKey));

                Assert.AreEqual("Primary encryption failed", ex.Message);

                this.mockPrimaryProvider.Verify(p => p.Encrypt(data, primaryKey), Times.Once);
                this.mockSecondaryProvider.Verify(p => p.Encrypt(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
            }

            [Test]
            public void Decrypt_String_WithInvalidKeys_ShouldPropagateExceptions()
            {
                // Arrange
                var encryptedData = "SecondaryEncryptedData";
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };

                this.mockPrimaryProvider
                    .Setup(p => p.Decrypt(encryptedData, primaryKey))
                    .Throws(new CryptographicException("Primary decryption failed"));

                // Act & Assert
                var ex = Assert.Throws<CryptographicException>(() =>
                    this.doubleProvider.Decrypt(encryptedData, primaryKey, secondaryKey));

                Assert.AreEqual("Primary decryption failed", ex.Message);

                this.mockPrimaryProvider.Verify(p => p.Decrypt(encryptedData, primaryKey), Times.Once);
                this.mockSecondaryProvider.Verify(p => p.Decrypt(It.IsAny<string>(), It.IsAny<byte[]>()), Times.Never);
            }

            [Test]
            public void Encrypt_String_WithEmptyString_ShouldEncryptCorrectly()
            {
                // Arrange
                var data = string.Empty;
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };

                var primaryEncrypted = "PrimaryEncryptedEmpty";
                var secondaryEncrypted = "SecondaryEncryptedEmpty";

                this.mockPrimaryProvider
                    .Setup(p => p.Encrypt(data, primaryKey))
                    .Returns(primaryEncrypted)
                    .Verifiable();

                this.mockSecondaryProvider
                    .Setup(p => p.Encrypt(primaryEncrypted, secondaryKey))
                    .Returns(secondaryEncrypted)
                    .Verifiable();

                // Act
                var result = this.doubleProvider.Encrypt(data, primaryKey, secondaryKey);

                // Assert
                Assert.AreEqual(secondaryEncrypted, result);

                this.mockPrimaryProvider.Verify(p => p.Encrypt(data, primaryKey), Times.Once);
                this.mockSecondaryProvider.Verify(p => p.Encrypt(primaryEncrypted, secondaryKey), Times.Once);
            }

            [Test]
            public void Decrypt_String_WithEmptyString_ShouldDecryptCorrectly()
            {
                // Arrange
                var encryptedData = "SecondaryEncryptedEmpty";
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };

                var primaryDecrypted = "PrimaryDecryptedEmpty";
                var finalDecrypted = string.Empty;

                this.mockPrimaryProvider
                    .Setup(p => p.Decrypt(encryptedData, primaryKey))
                    .Returns(primaryDecrypted)
                    .Verifiable();

                this.mockSecondaryProvider
                    .Setup(p => p.Decrypt(primaryDecrypted, secondaryKey))
                    .Returns(finalDecrypted)
                    .Verifiable();

                // Act
                var result = this.doubleProvider.Decrypt(encryptedData, primaryKey, secondaryKey);

                // Assert
                Assert.AreEqual(finalDecrypted, result);

                this.mockPrimaryProvider.Verify(p => p.Decrypt(encryptedData, primaryKey), Times.Once);
                this.mockSecondaryProvider.Verify(p => p.Decrypt(primaryDecrypted, secondaryKey), Times.Once);
            }

            [Test]
            public void Encrypt_String_WithSameInput_ShouldReturnConsistentEncryptedOutput()
            {
                // Arrange
                var data = "Consistent Data";
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };

                var primaryEncrypted = "PrimaryEncryptedConsistent";
                var secondaryEncrypted = "SecondaryEncryptedConsistent";

                this.mockPrimaryProvider
                    .Setup(p => p.Encrypt(data, primaryKey))
                    .Returns(primaryEncrypted)
                    .Verifiable();

                this.mockSecondaryProvider
                    .Setup(p => p.Encrypt(primaryEncrypted, secondaryKey))
                    .Returns(secondaryEncrypted)
                    .Verifiable();

                // Act
                var result1 = this.doubleProvider.Encrypt(data, primaryKey, secondaryKey);
                var result2 = this.doubleProvider.Encrypt(data, primaryKey, secondaryKey);

                // Assert
                Assert.AreEqual(secondaryEncrypted, result1);
                Assert.AreEqual(secondaryEncrypted, result2);

                this.mockPrimaryProvider.Verify(p => p.Encrypt(data, primaryKey), Times.Exactly(2));
                this.mockSecondaryProvider.Verify(p => p.Encrypt(primaryEncrypted, secondaryKey), Times.Exactly(2));
            }

            [Test]
            public void Decrypt_String_WithSameEncryptedInput_ShouldReturnConsistentDecryptedOutput()
            {
                // Arrange
                var encryptedData = "SecondaryEncryptedConsistent";
                var primaryKey = new byte[] { 1, 2, 3 };
                var secondaryKey = new byte[] { 4, 5, 6 };

                var primaryDecrypted = "PrimaryDecryptedConsistent";
                var finalDecrypted = "Consistent Data";

                this.mockPrimaryProvider
                    .Setup(p => p.Decrypt(encryptedData, primaryKey))
                    .Returns(primaryDecrypted)
                    .Verifiable();

                this.mockSecondaryProvider
                    .Setup(p => p.Decrypt(primaryDecrypted, secondaryKey))
                    .Returns(finalDecrypted)
                    .Verifiable();

                // Act
                var result1 = this.doubleProvider.Decrypt(encryptedData, primaryKey, secondaryKey);
                var result2 = this.doubleProvider.Decrypt(encryptedData, primaryKey, secondaryKey);

                // Assert
                Assert.AreEqual(finalDecrypted, result1);
                Assert.AreEqual(finalDecrypted, result2);

                this.mockPrimaryProvider.Verify(p => p.Decrypt(encryptedData, primaryKey), Times.Exactly(2));
                this.mockSecondaryProvider.Verify(p => p.Decrypt(primaryDecrypted, secondaryKey), Times.Exactly(2));
            }
        }

        #endregion
    


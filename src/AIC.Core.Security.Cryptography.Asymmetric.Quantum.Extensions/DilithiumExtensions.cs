namespace AIC.Core.Security.Cryptography.Asymmetric.Quantum.Extensions;

using System.Text;
using Newtonsoft.Json;
using Org.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;

public static class DilithiumExtensions
{
    private static byte[] salt =
    {
        0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0, 0x12, 0x34, 0x56, 0x78, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC,
        0xDE, 0xF0, 0x12, 0x34, 0x56, 0x78, 0x9A, 0xBC, 0xDE, 0xF0, 0x12, 0x34, 0x56, 0x78
    };

    public static byte[] GetDilithiumPrivateKeyBytes(this DilithiumPrivateKeyParameters privateKeyParameters)
    {
        var key = JsonConvert.SerializeObject(privateKeyParameters);

        return Encoding.UTF8.GetBytes(key);
    }

    public static byte[] GetDilithiumPublicKeyBytes(this DilithiumPublicKeyParameters publicKeyParameters)
    {
        var key = JsonConvert.SerializeObject(publicKeyParameters);

        return Encoding.UTF8.GetBytes(key);
    }

    public static DilithiumPrivateKeyParameters GetDilithiumPrivateKeyParameters(
        this byte[] dilithiumPrivateKeyBytes)
    {
        var json = Encoding.UTF8.GetString(dilithiumPrivateKeyBytes);

        return JsonConvert.DeserializeObject<DilithiumPrivateKeyParameters>(json);
    }

    public static DilithiumPublicKeyParameters GetDilithiumPublicKeyParameters(this byte[] dilithiumPublicKeyBytes)
    {
        var json = Encoding.UTF8.GetString(dilithiumPublicKeyBytes);

        return JsonConvert.DeserializeObject<DilithiumPublicKeyParameters>(json);
    }
}
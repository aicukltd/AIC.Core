namespace AIC.Core.Extensions;

using System.Globalization;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

public static class ByteArrayExtensions
{
    public static byte[] GetSubSetOfBytes(this byte[] bytes, int length, int startPosition)
    {
        var destinationArray = new byte[length];
        Array.Copy(bytes, startPosition, destinationArray, 0, length);
        return destinationArray;
    }

    public static byte[] HexBytesToBinary(this string hex)
    {
        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < hex.Length; i += 2) bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        return bytes;
    }

    public static string BytesToHex(this byte[] bytes)
    {
        var hex = new StringBuilder(bytes.Length * 2);
        foreach (var @byte in bytes) hex.Append($"{@byte:x2}");
        return hex.ToString();
    }

    public static byte[] PrivateKeyToBytes(this string privateKeyHex)
    {
        var privateSeed = BigInteger.Parse(
            privateKeyHex,
            NumberStyles.AllowHexSpecifier
        ).ToByteArray().Reverse().ToArray();
        return privateSeed;
    }

    public static string PrivateSeedToHex(this byte[] privateSeed)
    {
        var privateKey = new BigInteger(privateSeed.Reverse().ToArray());
        return privateKey.ToString("X").ToLower();
    }

    [Obsolete("BinaryFormatter.Serialize is obsolete in NET 7.0")]
    public static byte[] Serialize<T>(this T data)
    {
        var formatter = new BinaryFormatter();
        using var stream = new MemoryStream();
        formatter.Serialize(stream, data);
        return stream.ToArray();
    }

    [Obsolete("BinaryFormatter.Deserialize is obsolete in NET 7.0")]
    public static T Deserialize<T>(this byte[] array)
    {
        var stream = new MemoryStream(array);
        var formatter = new BinaryFormatter();
        return (T)formatter.Deserialize(stream);
    }

    public static int IndexOf<T>(this IEnumerable<T> source, T value)
    {
        var index = 0;
        var comparer = EqualityComparer<T>.Default; // or pass in as a parameter
        foreach (var item in source)
        {
            if (comparer.Equals(item, value)) return index;
            index++;
        }

        return -1;
    }

    public static int IndexOfByteArrayInByteArrayEnumerable(this IEnumerable<byte[]> source, byte[] value)
    {
        var index = 0;
        foreach (var item in source)
        {
            if (item.SequenceEqual(value)) return index;
            index++;
        }

        return -1;
    }

    public static byte[] PrependByteArray(this byte[] prefix, byte[] original)
    {
        var result = new byte[prefix.Length + original.Length];
        Array.Copy(prefix, 0, result, 0, prefix.Length);
        Array.Copy(original, 0, result, prefix.Length, original.Length);
        return result;
    }
}
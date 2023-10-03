using Frosty.Sdk.Utils;

namespace Frosty.Sdk.IO.Compression;

public interface ICompressionFormat
{
    public string Identifier { get; }
    
    /// <summary>
    /// Decompresses <see cref="inData"/> and writes it to <see cref="outData"/>.
    /// </summary>
    public void Decompress<T>(Block<T> inData, ref Block<T> outData, CompressionFlags inFlags = CompressionFlags.None) where T : unmanaged;
    
    /// <summary>
    /// Compresses <see cref="inData"/> and writes it to <see cref="outData"/>.
    /// </summary>
    public void Compress<T>(Block<T> inData, ref Block<T> outData, CompressionFlags inFlags = CompressionFlags.None) where T : unmanaged;
}   
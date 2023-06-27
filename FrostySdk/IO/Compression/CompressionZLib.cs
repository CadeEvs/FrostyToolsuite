using Frosty.Sdk.Utils;

namespace Frosty.Sdk.IO.Compression;

public class CompressionZLib : ICompressionFormat
{
    public string Identifier => "ZLib";
    
    public void Decompress<T>(Block<T> inData, ref Block<T> outData, CompressionFlags inFlags = CompressionFlags.None) where T : unmanaged
    {
        throw new System.NotImplementedException();
    }

    public void Compress<T>(Block<T> inData, ref Block<T> outData, CompressionFlags inFlags = CompressionFlags.None) where T : unmanaged
    {
        throw new System.NotImplementedException();
    }
}
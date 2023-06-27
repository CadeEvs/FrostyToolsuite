using Frosty.Sdk.Utils;

namespace Frosty.Sdk.IO.Compression;

public class CompressionOodle : ICompressionFormat
{
    public string Identifier => "Oodle";
    
    public void Decompress<T>(Block<T> inData, ref Block<T> outData, CompressionFlags inFlags = CompressionFlags.None) where T : unmanaged
    {
        throw new System.NotImplementedException();
    }

    public void Compress<T>(Block<T> inData, ref Block<T> outData, CompressionFlags inFlags = CompressionFlags.None) where T : unmanaged
    {
        throw new System.NotImplementedException();
    }
}
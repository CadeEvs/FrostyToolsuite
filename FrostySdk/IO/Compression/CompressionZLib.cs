using Frosty.Sdk.Utils;
using System;
using System.Runtime.InteropServices;

namespace Frosty.Sdk.IO.Compression;

public partial class CompressionZLib : ICompressionFormat
{
    public string Identifier => "ZLib";
    private const string NativeLibName = "zlib";
    
    [LibraryImport(NativeLibName)] internal static partial int compress(IntPtr dest, ulong destLen, IntPtr source, ulong sourceLen);
    [LibraryImport(NativeLibName)] internal static partial int uncompress(IntPtr dst, IntPtr dstCapacity, IntPtr source, ulong compressedSize);
    [LibraryImport(NativeLibName)] internal static partial IntPtr zError(int code);

    public unsafe void Decompress<T>(Block<T> inData, ref Block<T> outData, CompressionFlags inFlags = CompressionFlags.None) where T : unmanaged
    {
        int destCapacity = outData.Size;
        int err = uncompress((IntPtr)outData.Ptr, new IntPtr(&destCapacity), (IntPtr)inData.Ptr, (ulong)inData.Size);
        Error(err);
    }
    
    public unsafe void Compress<T>(Block<T> inData, ref Block<T> outData, CompressionFlags inFlags = CompressionFlags.None) where T : unmanaged
    {
        int err = compress((IntPtr)outData.Ptr, (ulong)outData.Size, (IntPtr)inData.Ptr, (ulong)inData.Size);
        Error(err);
    }

    private unsafe void Error(int code)
    {
        if (code != 0)
        {
            string error = new((sbyte*)zError(code));
            throw new Exception(error);
        }
    }
}
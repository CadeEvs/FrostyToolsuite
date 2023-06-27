using System;
using System.Runtime.InteropServices;
using Frosty.Sdk.Managers;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.IO.Compression;

public partial class CompressionZStd : ICompressionFormat
{
    private const string ZStdLibName = "zstd";
    private static IntPtr s_DictContext = IntPtr.Zero;
    
    [LibraryImport(ZStdLibName)] private static partial IntPtr ZSTD_decompress(IntPtr dst, IntPtr dstCapacity, IntPtr src, IntPtr compressedSize);
    [LibraryImport(ZStdLibName)] private static partial IntPtr ZSTD_createDCtx();
    [LibraryImport(ZStdLibName)] private static partial IntPtr ZSTD_decompress_usingDict(IntPtr dctx, IntPtr dst, IntPtr dstCapacity, IntPtr src, IntPtr srcSize, IntPtr dict, IntPtr dictSize);
    [LibraryImport(ZStdLibName)] private static partial IntPtr ZSTD_decompressDCtx(IntPtr dctx, IntPtr dst, IntPtr dstCapacity, IntPtr src, IntPtr srcSize);
    [LibraryImport(ZStdLibName)] private static partial IntPtr ZSTD_compress(IntPtr dst, IntPtr dstCapacity, IntPtr src, IntPtr srcSize);
    [LibraryImport(ZStdLibName)] private static partial IntPtr ZSTD_getErrorName(IntPtr code);
    [LibraryImport(ZStdLibName)] private static partial IntPtr ZSTD_isError(IntPtr code);
    
    public string Identifier => "ZStandard";

    /// <summary>
    /// Checks if the specified code is a valid ZStd error.
    /// </summary>
    /// 
    private static unsafe void GetError(IntPtr code)
    {
        if (ZSTD_isError(code) != IntPtr.Zero)
        {
            string error = new((sbyte*)ZSTD_getErrorName(code));
            throw new Exception($"A ZStandard operation failed with error \"{error}\"");
        }
    }
    
    public unsafe void Decompress<T>(Block<T> inData, ref Block<T> outData, CompressionFlags inFlags = CompressionFlags.None) where T : unmanaged
    {
        IntPtr code;
        if (inFlags.HasFlag(CompressionFlags.ZStdUseDicts))
        {
            // Create the dictionary context and load the dictionary if it wasn't already,
            // otherwise just decompress using the cached dict.
            if (s_DictContext == IntPtr.Zero)
            {
                s_DictContext = ZSTD_createDCtx();
                Block<byte> dict = FileSystemManager.GetFileFromMemoryFs("Dictionaries/ebx.dict");

                code = ZSTD_decompress_usingDict(s_DictContext, 
                    (IntPtr)outData.Ptr, outData.Size, 
                    (IntPtr)inData.Ptr, inData.Size, 
                    (IntPtr)dict.Ptr, dict.Size);
                
            }
            else
            {
                code = ZSTD_decompressDCtx(s_DictContext, 
                    (IntPtr)outData.Ptr, outData.Size, 
                    (IntPtr)inData.Ptr, inData.Size);
            }
        }
        else
        {
            code = ZSTD_decompress((IntPtr)outData.Ptr, outData.Size, (IntPtr)inData.Ptr, inData.Size);
        }
        GetError(code);
    }
    
    public unsafe void Compress<T>(Block<T> inData, ref Block<T> outData, CompressionFlags inFlags = CompressionFlags.None) where T : unmanaged
    {
        IntPtr code = ZSTD_compress((IntPtr)outData.Ptr, outData.Size, (IntPtr)inData.Ptr, inData.Size);
        GetError(code);
    }
}
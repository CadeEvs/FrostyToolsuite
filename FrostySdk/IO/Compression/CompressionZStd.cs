using System;
using System.Runtime.InteropServices;
using Frosty.Sdk.Managers;
using Frosty.Sdk.Utils;

namespace Frosty.Sdk.IO.Compression;

public partial class CompressionZStd : ICompressionFormat
{
    public string Identifier => "ZStandard";
    private const string NativeLibName = "zstd";
    
    private static IntPtr s_DDict = IntPtr.Zero;
    
    [LibraryImport(NativeLibName)] internal static partial IntPtr ZSTD_getErrorName(IntPtr code);
    [LibraryImport(NativeLibName)] internal static partial IntPtr ZSTD_isError(IntPtr code);
    [LibraryImport(NativeLibName)] internal static partial IntPtr ZSTD_createDDict(IntPtr dict, IntPtr dictSize);
    [LibraryImport(NativeLibName)] internal static partial IntPtr ZSTD_createDCtx();
    [LibraryImport(NativeLibName)] internal static partial IntPtr ZSTD_freeDCtx(IntPtr dctx);
    [LibraryImport(NativeLibName)] internal static partial IntPtr ZSTD_decompress(IntPtr dst, IntPtr dstCapacity, IntPtr src, IntPtr compressedSize);
    [LibraryImport(NativeLibName)] internal static partial IntPtr ZSTD_decompress_usingDDict(IntPtr dctx, IntPtr dst, IntPtr dstCapacity, IntPtr src, IntPtr srcSize, IntPtr dict);
    [LibraryImport(NativeLibName)] internal static partial IntPtr ZSTD_compress(IntPtr dst, IntPtr dstCapacity, IntPtr src, IntPtr srcSize);

    /// <summary>
    /// Checks if the specified code is a valid ZStd error.
    /// </summary>
    private unsafe static void GetError(IntPtr code)
    {
        if (ZSTD_isError(code) != IntPtr.Zero)
        {
            string error = new((sbyte*)ZSTD_getErrorName(code));
            throw new Exception($"A ZStandard operation failed with error: \"{error}\"");
        }
    }
    
    /// <inheritdoc/>
    public unsafe void Decompress<T>(Block<T> inData, ref Block<T> outData, CompressionFlags inFlags = CompressionFlags.None) where T : unmanaged
    {
        IntPtr code;
        if (inFlags.HasFlag(CompressionFlags.ZStdUseDicts))
        {
            if (s_DDict == IntPtr.Zero)
            {
                Block<byte> ebxDict = FileSystemManager.GetFileFromMemoryFs("Dictionaries/ebx.dict");
                s_DDict = ZSTD_createDDict((IntPtr)ebxDict.Ptr, ebxDict.Size);
            }

            IntPtr dctx = ZSTD_createDCtx();
            code = ZSTD_decompress_usingDDict(dctx, (IntPtr)outData.Ptr, outData.Size, (IntPtr)inData.Ptr, inData.Size, s_DDict);
            ZSTD_freeDCtx(dctx);
        }
        else
        {
            code = ZSTD_decompress((IntPtr)outData.Ptr, outData.Size, (IntPtr)inData.Ptr, inData.Size);
        }
        GetError(code);
    }
    
    /// <inheritdoc/>
    public unsafe void Compress<T>(Block<T> inData, ref Block<T> outData, CompressionFlags inFlags = CompressionFlags.None) where T : unmanaged
    {
        IntPtr code = ZSTD_compress((IntPtr)outData.Ptr, outData.Size, (IntPtr)inData.Ptr, inData.Size);
        GetError(code);
    }
}
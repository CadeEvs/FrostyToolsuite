using System;
using System.IO;
using System.Runtime.InteropServices;
using Frosty.Sdk.Managers;

namespace Frosty.Sdk.Utils.CompressionTypes;


public static class Oodle
{
    public enum OodleFormat : uint
    {
        LZH = 0,
        LZHLW = 1,
        LZNIB = 2,
        None = 3,
        LZB16 = 4,
        LZBLW = 5,
        LZA = 6,
        LZNA = 7,
        Kraken = 8,
        Mermaid = 9,
        BitKnit = 10,
        Selkie = 11,
        Hydra = 12,
        Leviathan = 13
    }

    public enum OodleCompressionLevel : uint
    {
        None = 0,
        SuperFast = 1,
        VeryFast = 2,
        Fast = 3,
        Normal = 4,
        Optimal1 = 5,
        Optimal2 = 6,
        Optimal3 = 7
    }

    public enum OodleFuzzSafe
    {
        No = 0,
        Yes = 1
    }

    public enum OodleCheckCRC
    {
        No = 0,
        Yes = 1
    }

    public enum OodleVerbosity
    {
        None = 0,
        Minimal = 1,
        Some = 2,
        Lots = 3
    }

    public enum OodleThreadPhase
    {
        ThreadPhase1 = 1,
        ThreadPhase2 = 2,
        ThreadPhaseAll = 3,
        Unthreaded = ThreadPhaseAll
    }

    public delegate int DecompressFunc(IntPtr srcBuffer, long srcSize, IntPtr dstBuffer, long dstSize, OodleFuzzSafe fuzzSafe = OodleFuzzSafe.Yes, OodleCheckCRC checkCRC = OodleCheckCRC.No, OodleVerbosity verbosity = OodleVerbosity.None, IntPtr decBufBase = new(), long decBufSize = 0, IntPtr fpCallback = new(), IntPtr callbackUserData = new(), IntPtr decoderMemory = new(), long decoderMemorySize = 0, OodleThreadPhase threadModule = OodleThreadPhase.Unthreaded);
    public static DecompressFunc Decompress;

    public delegate ulong CompressFunc(OodleFormat cmpCode, IntPtr srcBuffer, long srcSize, IntPtr cmpBuffer, OodleCompressionLevel cmpLevel, IntPtr options = new(), IntPtr dictionaryBase = new(), IntPtr lrm = new(), IntPtr scratch = new(), long scratchSize = 0);
    public static CompressFunc Compress;

    public delegate IntPtr GetOptionsFunc(OodleFormat cmpCode, OodleCompressionLevel cmpLevel);
    public static GetOptionsFunc GetOptions;

    public delegate long GetCompressedBufferSizeNeededFunc(long size);

    public static GetCompressedBufferSizeNeededFunc GetCompressedBufferSizeNeeded;
    
    public static bool TryBind()
    {
        DirectoryInfo di = new(FileSystemManager.BasePath);

        foreach (FileInfo fileInfo in di.EnumerateFiles())
        {
            if (fileInfo.Name.Contains("oodle", StringComparison.OrdinalIgnoreCase))
            {
                IntPtr libraryHandle = NativeLibrary.Load(fileInfo.FullName);

                if (NativeLibrary.TryGetExport(libraryHandle, "OodleLZ_Compress", out IntPtr compressHandle))
                {
                    Compress = Marshal.GetDelegateForFunctionPointer<CompressFunc>(compressHandle);
                }

                if (NativeLibrary.TryGetExport(libraryHandle, "OodleLZ_Decompress", out IntPtr decompressHandle))
                {
                    Decompress = Marshal.GetDelegateForFunctionPointer<DecompressFunc>(decompressHandle);
                }

                if (NativeLibrary.TryGetExport(libraryHandle, "OodleLZ_CompressOptions_GetDefault", out IntPtr optionsHandle))
                {
                    GetOptions = Marshal.GetDelegateForFunctionPointer<GetOptionsFunc>(optionsHandle);
                }

                if (NativeLibrary.TryGetExport(libraryHandle, "OodleLZ_GetCompressedBufferSizeNeeded", out IntPtr sizeNeededHandle))
                {
                    GetCompressedBufferSizeNeeded = Marshal.GetDelegateForFunctionPointer<GetCompressedBufferSizeNeededFunc>(sizeNeededHandle);
                }
                
                return true;
            }
        }

        return false;
    }
}
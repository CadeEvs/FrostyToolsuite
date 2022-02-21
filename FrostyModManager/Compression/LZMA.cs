using System;
using System.Collections.Generic;
//using SevenZip.Compression.LZMA;
using System.IO;
using FrostySdk.IO;
using System.Collections;
using System.Runtime.InteropServices;

namespace FrostyModManager.Compression
{
    static class LZMA
    {
        [DllImport("thirdparty/LZMA.dll", EntryPoint = "LzmaUncompress", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int LzmaUncompress(IntPtr dest, ref ulong destLen, IntPtr src, ref ulong srcLen, IntPtr props, ulong propsSize);

        [DllImport("thirdparty/LZMA.dll", EntryPoint = "Lzma2Uncompress", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        public static extern int Lzma2Uncompress(IntPtr dest, ref ulong destLen, IntPtr src, ref ulong srcLen, byte prop);
    }

    enum SevenZipProperty
    {
        kEnd=0,
        kHeader=1,
        kArchiveProperties=2,
        kAdditionalStreamsInfo=3,
        kMainStreamsInfo=4,
        kFilesInfo=5,
        kPackInfo=6,
        kUnPackInfo=7,
        kSubStreamsInfo=8,
        kSize=9,
        kCRC=10,
        kFolder=11,
        kCodersUnPackSize=12,
        kNumUnPackStream=13,
        kEmptyStream=14,
        kEmptyFile=15,
        kAnti=16,
        kNames=17,
        kCTime=18,
        kATime=19,
        kMTime=20,
        kWinAttributes=21,
        kComment=22,
        kEncodedHeader=23,
        kStartPos=24,
        kDummy=25
    };

    static class SevenZipUtils
    {
        public static ulong ReadEncodedULong(NativeReader reader)
        {
            byte firstByte = reader.ReadByte();
            byte numAdditional = 0;
            while ((firstByte & 0x80) != 0)
            {
                numAdditional++;
                firstByte <<= 1;
            }
            firstByte >>= numAdditional;

            ulong tmp = 0;
            for (int i = 0; i < numAdditional; i++)
                tmp += (ulong)reader.ReadByte() << (8 * i);

            return (((ulong)firstByte) << (8 * numAdditional)) + tmp;
        }

        public static string ReadNullTerminatedWideString(NativeReader reader)
        {
            string result = "";
            while (true)
            {
                char c = reader.ReadWideChar();
                if (c == 0x00)
                    return result;

                result += c;
            }
        }
    }

    class SevenZipBase
    {
        public virtual void Read(NativeReader reader)
        {
            throw new NotImplementedException();
        }
    }

    class SevenZipPackInfo : SevenZipBase
    {
        public ulong Pos { get; private set; }
        public ulong[] Sizes { get; private set; }

        private ulong numPackStreams;

        public override void Read(NativeReader reader)
        {
            Pos = SevenZipUtils.ReadEncodedULong(reader);
            numPackStreams = SevenZipUtils.ReadEncodedULong(reader);
            Sizes = new ulong[numPackStreams];

            SevenZipProperty prop = (SevenZipProperty)reader.ReadByte();
            while (prop != SevenZipProperty.kEnd)
            {
                switch (prop)
                {
                    case SevenZipProperty.kSize:
                        for (ulong i = 0; i < numPackStreams; i++)
                            Sizes[i] = SevenZipUtils.ReadEncodedULong(reader);
                        break;
                    case SevenZipProperty.kCRC:
                        break;
                }

                prop = (SevenZipProperty)reader.ReadByte();
            }
        }
    }

    class SevenZipFolder
    {
        public FolderCoder[] Coders { get; private set; }
        public ulong NumOutStreamsTotal { get; private set; }
        public ulong NumInStreamsTotal { get; private set; }

        public struct Flags
        {
            internal byte data;
            public Flags(byte inData)
            {
                data = inData;
            }

            public int CodecIdSize => (data & 0x0F);
            public bool IsComplexCoder => (data & 0x10) != 0;
            public bool HasAttributes => (data & 0x20) != 0;
        }
        public struct FolderCoder
        {
            public Flags flags;
            public byte[] codecId;
            public ulong numInStreams;
            public ulong numOutStreams;
            public byte[] properties;
        }

        Tuple<ulong, ulong>[] bindPairs;

        public void Read(NativeReader reader)
        {
            ulong numCoders = SevenZipUtils.ReadEncodedULong(reader);
            Coders = new FolderCoder[numCoders];

            NumOutStreamsTotal = 0;
            NumInStreamsTotal = 0;

            for (ulong i = 0; i < numCoders; i++)
            {
                Coders[i] = new FolderCoder {flags = new Flags(reader.ReadByte())};
                Coders[i].codecId = reader.ReadBytes(Coders[i].flags.CodecIdSize);
                Coders[i].numOutStreams = 1;

                if(Coders[i].flags.IsComplexCoder)
                {
                    Coders[i].numInStreams = SevenZipUtils.ReadEncodedULong(reader);
                    Coders[i].numOutStreams = SevenZipUtils.ReadEncodedULong(reader);
                }
                if (Coders[i].flags.HasAttributes)
                {
                    ulong propertiesSize = SevenZipUtils.ReadEncodedULong(reader);
                    Coders[i].properties = reader.ReadBytes((int)propertiesSize);
                }

                NumInStreamsTotal += Coders[i].numInStreams;
                NumOutStreamsTotal += Coders[i].numOutStreams;
            }

            ulong numBindPairs = NumOutStreamsTotal - 1;
            bindPairs = new Tuple<ulong, ulong>[numBindPairs];
            for (ulong i = 0; i < numBindPairs; i++)
                bindPairs[i] = new Tuple<ulong, ulong>(SevenZipUtils.ReadEncodedULong(reader), SevenZipUtils.ReadEncodedULong(reader));

            ulong numPackedStreams = NumInStreamsTotal - numBindPairs;
            if (numPackedStreams > 1)
            {
                for (ulong i = 0; i < numPackedStreams; i++)
                    SevenZipUtils.ReadEncodedULong(reader);
            }
        }
    }

    class SevenZipDigest
    {
        private bool allDefined;
        private uint[] crc;

        public SevenZipDigest(ulong count)
        {
            crc = new uint[count];
        }

        public void Read(NativeReader reader)
        {
            allDefined = reader.ReadByte() == 1;
            if (!allDefined)
            {
                // @todo
            }

            for (ulong i = 0; i < (ulong)crc.LongLength; i++)
                crc[i] = reader.ReadUInt();
        }
    }

    class SevenZipCodersInfo : SevenZipBase
    {
        public ulong[][] Sizes { get; private set; }
        public SevenZipFolder[] Folders { get; private set; }

        private ulong numFolders;
        private bool isExternal;
        private ulong externalSteamIndex;
        private SevenZipDigest crc;

        public override void Read(NativeReader reader)
        {
            SevenZipProperty prop = (SevenZipProperty)reader.ReadByte();
            while (prop != SevenZipProperty.kEnd)
            {
                switch (prop)
                {
                    case SevenZipProperty.kFolder:
                        numFolders = SevenZipUtils.ReadEncodedULong(reader);
                        isExternal = reader.ReadByte() == 0x01;
                        if (!isExternal)
                        {
                            Folders = new SevenZipFolder[numFolders];
                            for (ulong i = 0; i < numFolders; i++)
                            {
                                Folders[i] = new SevenZipFolder();
                                Folders[i].Read(reader);
                            }
                        }
                        else
                            externalSteamIndex = SevenZipUtils.ReadEncodedULong(reader);
                        break;
                    case SevenZipProperty.kCodersUnPackSize:
                        Sizes = new ulong[numFolders][];
                        for(ulong i = 0; i < numFolders; i++)
                        {
                            Sizes[i] = new ulong[Folders[i].NumOutStreamsTotal];

                            ulong total = 0;
                            for (ulong j = 0; j < (ulong)Folders[i].Coders.LongLength; j++)
                            {
                                for (ulong k = 0; k < Folders[i].Coders[j].numOutStreams; k++)
                                    Sizes[i][total++] = SevenZipUtils.ReadEncodedULong(reader);
                            }
                        }
                        break;
                    case SevenZipProperty.kCRC:
                        crc = new SevenZipDigest(numFolders);
                        crc.Read(reader);
                        break;
                }

                prop = (SevenZipProperty)reader.ReadByte();
            }
        }
    }

    class SevenZipSubStreamsInfo : SevenZipBase
    {
        public ulong[] UnpackStreamCount { get; }
        public ulong[][] Sizes { get; }
        public ulong[] TotalSizes { get; }

        SevenZipDigest[] crcs;

        public SevenZipSubStreamsInfo(ulong inNumFolders)
        {
            UnpackStreamCount = new ulong[inNumFolders];
            Sizes = new ulong[inNumFolders][];
            TotalSizes = new ulong[inNumFolders];

            crcs = new SevenZipDigest[inNumFolders];
        }

        public override void Read(NativeReader reader)
        {
            SevenZipProperty prop = (SevenZipProperty)reader.ReadByte();
            while (prop != SevenZipProperty.kEnd)
            {
                switch (prop)
                {
                    case SevenZipProperty.kNumUnPackStream:
                        for (int i = 0; i < UnpackStreamCount.Length; i++)
                            UnpackStreamCount[i] = SevenZipUtils.ReadEncodedULong(reader);
                        break;

                    case SevenZipProperty.kSize:
                        for (int i = 0; i < UnpackStreamCount.Length; i++)
                        {
                            Sizes[i] = new ulong[UnpackStreamCount[i]];
                            for (ulong j = 0; j < UnpackStreamCount[i] - 1; j++)
                            {
                                Sizes[i][j] = SevenZipUtils.ReadEncodedULong(reader);
                                TotalSizes[i] += Sizes[i][j];
                            }
                        }
                        break;

                    case SevenZipProperty.kCRC:
                        for (int i = 0; i < crcs.Length; i++)
                        {
                            crcs[i] = new SevenZipDigest(UnpackStreamCount[i]);
                            crcs[i].Read(reader);
                        }
                        break;
                }

                prop = (SevenZipProperty)reader.ReadByte();
            }
        }
    }

    class SevenZipStreamsInfo : SevenZipBase
    {
        public SevenZipPackInfo PackInfo { get; private set; }
        public SevenZipCodersInfo CodersInfo { get; private set; }
        public SevenZipSubStreamsInfo SubStreamsInfo { get; private set; }

        public override void Read(NativeReader reader)
        {
            SevenZipProperty prop = (SevenZipProperty)reader.ReadByte();
            while (prop != SevenZipProperty.kEnd)
            {
                switch (prop)
                {
                    case SevenZipProperty.kPackInfo:
                        PackInfo = new SevenZipPackInfo();
                        PackInfo.Read(reader);
                        break;
                    case SevenZipProperty.kUnPackInfo:
                        CodersInfo = new SevenZipCodersInfo();
                        CodersInfo.Read(reader);
                        break;
                    case SevenZipProperty.kSubStreamsInfo:
                        {
                            SubStreamsInfo = new SevenZipSubStreamsInfo((ulong)CodersInfo.Folders.LongLength);
                            SubStreamsInfo.Read(reader);

                            for (long i = 0; i < CodersInfo.Sizes.LongLength; i++)
                            {
                                ulong unpackSize = CodersInfo.Sizes[i][0];
                                if (SubStreamsInfo.UnpackStreamCount[i] > 0)
                                {
                                    SubStreamsInfo.Sizes[i][SubStreamsInfo.UnpackStreamCount[i] - 1] = unpackSize - SubStreamsInfo.TotalSizes[i];
                                }
                            }
                        }
                        break;
                }

                prop = (SevenZipProperty)reader.ReadByte();
            }
        }
    }

    class SevenZipArchiveProperties : SevenZipBase
    {
        public struct Property
        {
            public byte type;
            public byte[] data;
        }

        private List<Property> properties = new List<Property>();

        public override void Read(NativeReader reader)
        {
            byte type = reader.ReadByte();
            while (type != 0x00)
            {
                ulong size = SevenZipUtils.ReadEncodedULong(reader);

                Property prop = new Property
                {
                    type = type,
                    data = reader.ReadBytes((int)size)
                };

                properties.Add(prop);
            }
        }
    }

    class SevenZipFilesInfo : SevenZipBase
    {
        public string[] Filenames { get; private set; }

        public override void Read(NativeReader reader)
        {
            ulong numFiles = SevenZipUtils.ReadEncodedULong(reader);
            Filenames = new string[numFiles];

            SevenZipProperty prop = (SevenZipProperty)reader.ReadByte();
            while (prop != SevenZipProperty.kEnd)
            {
                switch (prop)
                {
                    case SevenZipProperty.kEmptyStream:
                        break;
                    case SevenZipProperty.kEmptyFile:
                        break;
                    case SevenZipProperty.kAnti:
                        break;
                    case SevenZipProperty.kCTime:
                        break;
                    case SevenZipProperty.kATime:
                        break;
                    case SevenZipProperty.kDummy:
                        ulong size = SevenZipUtils.ReadEncodedULong(reader);
                        reader.ReadBytes((int)size);
                        break;
                    case SevenZipProperty.kMTime:
                        {
                            bool allDefined = reader.ReadByte() == 1;
                            bool isExternal = reader.ReadByte() == 1;
                            for (ulong i = 0; i < numFiles; i++)
                                reader.ReadULong();
                        }
                        break;
                    case SevenZipProperty.kNames:
                        {
                            bool isExternal = reader.ReadByte() == 1;
                            if (isExternal)
                                SevenZipUtils.ReadEncodedULong(reader);
                            else
                            {
                                for (ulong i = 0; i < numFiles; i++)
                                    Filenames[i] = SevenZipUtils.ReadNullTerminatedWideString(reader);
                            }
                        }
                        break;
                    case SevenZipProperty.kWinAttributes:
                        {
                            bool allDefined = reader.ReadByte() == 1;
                            bool isExternal = reader.ReadByte() == 1;
                            for (ulong i = 0; i < numFiles; i++)
                                reader.ReadUInt();
                        }
                        break;
                    case SevenZipProperty.kStartPos:
                        prop = (SevenZipProperty)reader.ReadByte();
                        SevenZipUtils.ReadEncodedULong(reader);
                        continue;
                }

                prop = (SevenZipProperty)reader.ReadByte();
            }

        }
    }

    class SevenZipHeader : SevenZipBase
    {
        public SevenZipStreamsInfo MainStream { get; private set; }
        public SevenZipFilesInfo Files { get; private set; }

        private SevenZipArchiveProperties archiveProps;
        private SevenZipStreamsInfo additionalStreams;

        public override void Read(NativeReader reader)
        {
            SevenZipProperty prop = (SevenZipProperty)reader.ReadByte();
            while (prop != SevenZipProperty.kEnd)
            {
                switch (prop)
                {
                    case SevenZipProperty.kArchiveProperties:
                        archiveProps = new SevenZipArchiveProperties();
                        archiveProps.Read(reader);
                        break;
                    case SevenZipProperty.kAdditionalStreamsInfo:
                        additionalStreams = new SevenZipStreamsInfo();
                        additionalStreams.Read(reader);
                        break;
                    case SevenZipProperty.kMainStreamsInfo:
                        MainStream = new SevenZipStreamsInfo();
                        MainStream.Read(reader);
                        break;
                    case SevenZipProperty.kFilesInfo:
                        Files = new SevenZipFilesInfo();
                        Files.Read(reader);
                        break;
                }

                prop = (SevenZipProperty)reader.ReadByte();
            }
        }
    }

    public class SevenZipDecompressor : IDecompressor
    {
        private static byte[] kSignature = new byte[] { 0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C };
        private NativeReader reader;
        private SevenZipHeader header;
        private bool bProcessed;
        private long packedStreamsOffset;

        public bool OpenArchive(string filename)
        {
            reader = new NativeReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            byte[] signature = reader.ReadBytes(6);
            if (!StructuralComparisons.StructuralEqualityComparer.Equals(signature, kSignature))
                return false;

            byte majorVersion = reader.ReadByte();
            byte minorVersion = reader.ReadByte();

            uint startHeaderCRC = reader.ReadUInt();
            long nextHeaderOffset = reader.ReadLong();
            long nextHeaderSize = reader.ReadLong();
            uint nextHeaderCRC = reader.ReadUInt();

            packedStreamsOffset = reader.Position;
            reader.Position += nextHeaderOffset;

            header = new SevenZipHeader();
            SevenZipProperty prop = (SevenZipProperty)reader.ReadByte();

            if (prop == SevenZipProperty.kEncodedHeader)
            {
                SevenZipStreamsInfo encodedHeader = new SevenZipStreamsInfo();
                encodedHeader.Read(reader);

                ulong offset = encodedHeader.PackInfo.Pos;
                ulong compressedSize = encodedHeader.PackInfo.Sizes[0];
                ulong uncompressedSize = encodedHeader.CodersInfo.Sizes[0][0];

                byte[] codec = encodedHeader.CodersInfo.Folders[0].Coders[0].codecId;
                if (codec[0] == 0x03 && codec[1] == 0x01 && codec[2] == 0x01)
                {
                    reader.Position = packedStreamsOffset + (long)offset;

                    byte[] buffer = reader.ReadBytes((int)compressedSize);
                    byte[] uncompressedBuffer = Decompress(buffer, encodedHeader.CodersInfo.Folders[0].Coders[0].properties, compressedSize, uncompressedSize);

                    //using (NativeWriter writer = new NativeWriter(new FileStream("E:\\Temp\\output.bin", FileMode.Create)))
                    //    writer.Write(uncompressedBuffer);

                    using (NativeReader headerReader = new NativeReader(new MemoryStream(uncompressedBuffer)))
                        header.Read(headerReader);
                }
            }
            else if (prop == SevenZipProperty.kHeader)
            {
                header.Read(reader);
            }

            reader.Position = packedStreamsOffset + (long)header.MainStream.PackInfo.Pos;
            return true;
        }

        public void CloseArchive()
        {
            block.Dispose();
            block = null;

            reader.Dispose();
            reader = null;
        }

        public void DecompressToFile(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = DecompressToMemory();
                fs.Write(buffer, 0, buffer.Length);
            }
        }

        public byte[] DecompressToMemory()
        {
            byte[] buffer = new byte[currentLength];

            block.Read(buffer, 0, (int)currentLength);
            bProcessed = true;

            return buffer;
        }

        private string currentFilename;
        private ulong currentLength;

        private MemoryStream block;

        public IEnumerable<CompressedFileInfo> EnumerateFiles()
        {
            for (long folderId = 0; folderId < header.MainStream.CodersInfo.Folders.LongLength; folderId++)
            {
                if (block != null)
                {
                    // moving onto next folder
                    // expose of any existing blocks

                    block.Dispose();
                    block = null;
                }

                ulong compressedLength = header.MainStream.PackInfo.Sizes[folderId];
                ulong blockLength = header.MainStream.CodersInfo.Sizes[folderId][0];

                byte[] compBuf = reader.ReadBytes((int)compressedLength);
                byte[] buf = new byte[blockLength];
                byte[] props = header.MainStream.CodersInfo.Folders[folderId].Coders[0].properties;
                byte[] codec = header.MainStream.CodersInfo.Folders[folderId].Coders[0].codecId;

                if (codec[0] == 0x21)
                {
                    // LZMA2
                    GCHandle ptr1 = GCHandle.Alloc(buf, GCHandleType.Pinned);
                    GCHandle ptr2 = GCHandle.Alloc(compBuf, GCHandleType.Pinned);

                    LZMA.Lzma2Uncompress(ptr1.AddrOfPinnedObject(), ref blockLength, ptr2.AddrOfPinnedObject(), ref compressedLength, props[0]);

                    ptr1.Free();
                    ptr2.Free();
                }
                else if (codec[0] == 0x03 && codec[1] == 0x01 && codec[2] == 0x01)
                {
                    // LZMA
                    GCHandle ptr1 = GCHandle.Alloc(buf, GCHandleType.Pinned);
                    GCHandle ptr2 = GCHandle.Alloc(compBuf, GCHandleType.Pinned);
                    GCHandle ptr3 = GCHandle.Alloc(props, GCHandleType.Pinned);

                    LZMA.LzmaUncompress(ptr1.AddrOfPinnedObject(), ref blockLength, ptr2.AddrOfPinnedObject(), ref compressedLength, ptr3.AddrOfPinnedObject(), (ulong)props.Length);

                    ptr1.Free();
                    ptr2.Free();
                    ptr3.Free();
                }
                else
                {
                    // unsupported compression
                    yield break;
                }

                block = new MemoryStream(buf);
                bProcessed = true;

                for (int i = 0; i < header.MainStream.SubStreamsInfo.Sizes[folderId].Length; i++)
                {
                    if (!bProcessed)
                        block.Position += (long)currentLength;

                    bProcessed = false;
                    currentFilename = header.Files.Filenames[i];
                    currentLength = (header.MainStream.SubStreamsInfo.UnpackStreamCount[folderId] > 0)
                        ? header.MainStream.SubStreamsInfo.Sizes[folderId][i]
                        : blockLength;

                    yield return new CompressedFileInfo(currentFilename, 0, currentLength);
                }
            }
        }

        private static byte[] Decompress(byte[] inBuffer, byte[] props, ulong compressedSize, ulong uncompressedSize)
        {
            byte[] buf = new byte[uncompressedSize];

            GCHandle ptr1 = GCHandle.Alloc(buf, GCHandleType.Pinned);
            GCHandle ptr2 = GCHandle.Alloc(inBuffer, GCHandleType.Pinned);
            GCHandle ptr3 = GCHandle.Alloc(props, GCHandleType.Pinned);

            LZMA.LzmaUncompress(ptr1.AddrOfPinnedObject(), ref uncompressedSize, ptr2.AddrOfPinnedObject(), ref compressedSize, ptr3.AddrOfPinnedObject(), (ulong)props.Length);

            ptr1.Free();
            ptr2.Free();
            ptr3.Free();

            return buf;
        }
    }
}

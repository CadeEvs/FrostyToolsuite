using FrostySdk.IO;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace FrostyModManager.Compression
{
    public class ZipDecompressor : IDecompressor
    {
        private ZipArchive archive;
        private ZipArchiveEntry currentEntry;

        public bool OpenArchive(string filename)
        {
            archive = new ZipArchive(new FileStream(filename, FileMode.Open, FileAccess.Read), ZipArchiveMode.Read);
            return true;
        }

        public void CloseArchive()
        {
            archive.Dispose();
            archive = null;
            currentEntry = null;
        }

        public IEnumerable<CompressedFileInfo> EnumerateFiles()
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                currentEntry = entry;
                yield return new CompressedFileInfo(entry.FullName, (ulong)entry.CompressedLength, (ulong)entry.Length, entry.Open());
            }
            currentEntry = null;
        }

        public void DecompressToFile(string filename)
        {
            byte[] buffer = DecompressToMemory();
            using (NativeWriter writer = new NativeWriter(new FileStream(filename, FileMode.Create)))
                writer.Write(buffer);
        }

        public byte[] DecompressToMemory()
        {
            Stream stream = currentEntry.Open();
            using (MemoryStream ms = new MemoryStream())
            {
                long remainingLength = currentEntry.Length;
                while (remainingLength > 0)
                {
                    int bufferLength = (remainingLength > int.MaxValue) ? int.MaxValue : (int)remainingLength;
                    byte[] tmpBuffer = new byte[bufferLength];

                    stream.Read(tmpBuffer, 0, bufferLength);
                    ms.Write(tmpBuffer, 0, bufferLength);

                    remainingLength -= bufferLength;
                }

                return ms.ToArray();
            }
        }
    }

    public class ZipCompressor
    {
        ZipArchive archive;
        string name;

        public bool CreateArchive(string filename)
        {
            archive = ZipFile.Open(filename, ZipArchiveMode.Create);
            name = filename;
            return true;
        }

        public void AddEntry(string fileName)
        {
            archive.CreateEntryFromFile(name, fileName);
        }
    }
}

using System.Collections.Generic;
using System.IO;

namespace FrostyModManager.Compression
{
    public class CompressedFileInfo
    {
        public string Filename
        {
            get
            {
                int idx = filename.LastIndexOf('\\');
                return idx == -1 ? filename : filename.Substring(idx + 1);
            }
        }
        public string DirectoryName
        {
            get
            {
                int idx = filename.LastIndexOf('\\');
                return idx == -1 ? "" : filename.Substring(0, idx);
            }
        }
        public string Extension
        {
            get
            {
                int idx = Filename.LastIndexOf('.');
                return idx == -1 ? "" : Filename.Substring(idx);
            }
        }

        public Stream Stream => stream;

        private string filename;
        private ulong compressedSize;
        private ulong uncompressedSize;

        private Stream stream;
        public CompressedFileInfo(string inFilename, ulong inCompressedSize, ulong inUncompressedSize, Stream inStream = null)
        {
            filename = inFilename.Replace('/', '\\');
            compressedSize = inCompressedSize;
            uncompressedSize = inUncompressedSize;
            stream = inStream;
        }
    }

    public interface IDecompressor
    {
        bool OpenArchive(string filename);
        void CloseArchive();

        IEnumerable<CompressedFileInfo> EnumerateFiles();
        byte[] DecompressToMemory();
        void DecompressToFile(string filename);
    }
}

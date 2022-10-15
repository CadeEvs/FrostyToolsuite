using FrostySdk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrostySdk
{
    public struct Profile
    {
        public string Name;
        public string DisplayName;
        public IProfile ProfileData;
        public int DataVersion;
        public string CacheName;
        public string Deobfuscator;
        public string AssetLoader;
        public List<FileSystemSource> Sources;
        public string SDKFilename;
        public byte[] Banner;

        public int EbxVersion;
        public bool RequiresKey;
        public bool MustAddChunks;
        public bool EnableExecution;
        public bool ContainsEAC;

        public string DefaultDiffuse;
        public string DefaultNormals;
        public string DefaultMask;
        public string DefaultTint;

        public Dictionary<int, string> SharedBundles;
        public List<uint> IgnoredResTypes;
    }
}

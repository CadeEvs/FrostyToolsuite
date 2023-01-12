using FrostySdk;
using FrostySdk.IO;
using System.Collections.Generic;
using System.IO;
using Frosty.Core.IO;

namespace Frosty.Core.Mod
{
    public sealed class FrostyMod : IFrostyMod, IResourceContainer
    {
        /*
          Mod Format Versions:
            FBMOD   - Initial Version
            FBMODV1 - (Unknown)
            FBMODV2 - Special action added for chunks bundles

            1 - Start of new binary format
              - Support for custom data handlers (only for legacy files for now)
            2 - Merging of defined res files (eg. ShaderBlockDepot)
            3 - Added user data
            4 - Various structural changes as well as removal of modifiedBundles
            5 - Added link for the modpage
            6 - Added superbundle ids for toc chunks
        */

        public static ulong Magic = 0x01005954534F5246;
        public static uint Version = 6;

        public FrostyModDetails ModDetails { get; }
        public string Path { get; }
        public string Filename { get; }
        public int GameVersion { get; }

        public IEnumerable<string> Warnings => warnings;
        public bool HasWarnings => warnings.Count != 0;

        public bool NewFormat { get; } = false;

        public IEnumerable<BaseModResource> Resources => resources;

        private List<string> warnings = new List<string>();

        // new stuff
        private BaseModResource[] resources;

        /// <summary>
        /// Legacy constructor
        /// </summary>
        public FrostyMod(string inFilename, DbObject modObj)
        {
            FileInfo fi = new FileInfo(inFilename);
            Filename = fi.Name;
            Path = inFilename;

            ModDetails = new FrostyModDetails(
                modObj.GetValue<string>("title"),
                modObj.GetValue<string>("author"),
                modObj.GetValue<string>("category"),
                modObj.GetValue<string>("version"),
                modObj.GetValue<string>("description"),
                ""
                );
            GameVersion = modObj.GetValue<int>("gameVersion");

            DbObject resourceList = modObj.GetValue<DbObject>("resources");
            int iconResourceId = modObj.GetValue<int>("icon", -1);

            if (iconResourceId != -1)
                ModDetails.SetIcon(GetResource(resourceList, iconResourceId));

            foreach (int screenshotResourceId in modObj.GetValue<DbObject>("screenshots"))
                ModDetails.AddScreenshot(GetResource(resourceList, screenshotResourceId));
        }

        /// <summary>
        /// Constructor for new binary format
        /// </summary>
        public FrostyMod(string inFilename)
        {
            FileInfo fi = new FileInfo(inFilename);
            Filename = fi.Name;
            Path = inFilename;

            using (FrostyModReader reader = new FrostyModReader(new FileStream(inFilename, FileMode.Open, FileAccess.Read)))
            {
                if (reader.IsValid)
                {
                    NewFormat = true;

                    GameVersion = reader.GameVersion;
                    ModDetails = reader.ReadModDetails();

                    resources = reader.ReadResources();
                    ModDetails.SetIcon(reader.GetResourceData(resources[0]));

                    for (int i = 0; i < 4; i++)
                    {
                        byte[] buf = reader.GetResourceData(resources[i + 1]);
                        if (buf != null)
                            ModDetails.AddScreenshot(buf);
                    }
                }
            }
        }

        public byte[] GetResourceData(BaseModResource resource)
        {
            using (FrostyModReader reader = new FrostyModReader(new FileStream(Path, FileMode.Open, FileAccess.Read)))
                return reader.GetResourceData(resource);
        }

        public void AddWarning(string warning)
        {
            warnings.Add(warning);
        }

        private byte[] GetResource(DbObject resourceList, int resourceId)
        {
            byte[] outBuffer = null;
            DbObject resourceObj = resourceList[resourceId] as DbObject;
            int archiveIndex = resourceObj.GetValue<int>("archiveIndex");

            FileInfo fi = new FileInfo(Path);
            FileInfo archiveFi = new FileInfo(fi.FullName.Replace(".fbmod", "_" + archiveIndex.ToString("D2") + ".archive"));

            if (archiveFi.Exists)
            {
                using (NativeReader reader = new NativeReader(new FileStream(archiveFi.FullName, FileMode.Open, FileAccess.Read)))
                {
                    reader.Position = resourceObj.GetValue<long>("archiveOffset");
                    outBuffer = reader.ReadBytes(resourceObj.GetValue<int>("uncompressedSize"));
                }
            }

            return outBuffer;
        }
    }
}
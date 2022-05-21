using FrostySdk;
using FrostySdk.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace Frosty.Core.Mod
{
    public class CollectionManifest
    {
        public string link { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public string version { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public List<string> mods { get; set; }
        public List<string> modVersions { get; set; }
    }
    public sealed class FrostyModCollection : ISuperGamerLeagueGamer
    {
        public FrostyModDetails ModDetails { get; }
        public IEnumerable<string> Warnings
        {
            get
            {
                for (int i = 0; i < Mods.Count; i++)
                {
                    foreach (var warning in Mods[i].Warnings)
                        yield return warning;
                }
            }
        }
        public bool HasWarnings
        {
            get
            {
                for (int i = 0; i < Mods.Count; i++)
                {
                    if (Mods[i].HasWarnings)
                        return true;
                }
                return false;
            }
        }
        public string Filename { get; }
        public string Path { get; }
        public List<FrostyMod> Mods { get; }
        public bool IsValid { get; }

        private static readonly uint magic = 0x46434F4C;
        private static readonly uint version = 1;

        public FrostyModCollection(CollectionManifest manifest, List<FrostyMod> mods)
        {
            Filename = manifest.title + ".fbcollection";
            ModDetails = new FrostyModDetails(manifest.title, manifest.author, manifest.category, manifest.version, manifest.description, manifest.link);
            Mods = mods;
        }

        public FrostyModCollection(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            Filename = fi.Name;
            Path = fileName;
            using (NativeReader reader = new NativeReader(new FileStream(fileName, FileMode.Open, FileAccess.Read)))
            {
                uint inMagic = reader.ReadUInt();
                if (inMagic != magic)
                    throw new FileFormatException();

                uint inVersion = reader.ReadUInt();

                uint manifestOffset = reader.ReadUInt();
                int manifestSize = reader.ReadInt();
                uint iconOffset = reader.ReadUInt();
                int iconSize = reader.ReadInt();
                uint screenShotsOffset = reader.ReadUInt();

                reader.Position = manifestOffset;
                CollectionManifest manifest = null;
                using (StreamReader sreader = new StreamReader(reader.CreateViewStream(manifestOffset, manifestSize)))
                    manifest = JsonConvert.DeserializeObject<CollectionManifest>(sreader.ReadToEnd());
                ModDetails = new FrostyModDetails(manifest.title, manifest.author, manifest.category, manifest.version, manifest.description, manifest.link);

                reader.Position = iconOffset;
                ModDetails?.SetIcon(reader.ReadBytes(iconSize));

                reader.Position = screenShotsOffset;
                int count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    int size = reader.ReadInt();
                    ModDetails?.AddScreenshot(reader.ReadBytes(size));
                }

                Mods = new List<FrostyMod>(manifest.mods.Count);
                for (int i = 0; i < manifest.mods.Count; i++)
                {
                    fi = new FileInfo(fi.DirectoryName + "/" + manifest.mods[i]);
                    if (fi.Exists)
                    {
                        FrostyMod fmod = new FrostyMod(fi.FullName);
                        if (!fmod.NewFormat)
                        {
                            using (DbReader dbReader = new DbReader(new FileStream(fi.FullName, FileMode.Open, FileAccess.Read), null))
                                fmod = new FrostyMod(fi.FullName, dbReader.ReadDbObject());
                        }

                        if (fmod.ModDetails.Version == manifest.modVersions[i])
                            Mods.Add(fmod);
                    }
                    else
                        throw new Exception("Mod " + manifest.mods[i] + " is missing.");

                }
                IsValid = true;
            }
        }

        public byte[] WriteCollection()
        {
            using (NativeWriter writer = new NativeWriter(new MemoryStream()))
            {
                writer.Write(magic);
                writer.Write(version);

                writer.Write(0xDEADBEEF);
                writer.Write(0xDEADBEEF);
                writer.Write(0xDEADBEEF);
                writer.Write(0xDEADBEEF);
                writer.Write(0xDEADBEEF);

                CollectionManifest manifest = new CollectionManifest()
                {
                    title = ModDetails.Title,
                    author = ModDetails.Author,
                    category = ModDetails.Category,
                    version = ModDetails.Version,
                    description = ModDetails.Description,
                    link = ModDetails.Link
                };
                manifest.mods = new List<string>(Mods.Count);
                manifest.modVersions = new List<string>(Mods.Count);
                foreach (var mod in Mods)
                {
                    manifest.mods.Add(mod.Filename);
                    manifest.modVersions.Add(mod.ModDetails.Version);
                }

                uint manifestOffset = (uint)writer.Position;
                writer.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(manifest, Formatting.Indented)));
                int manifestSize = (int)(writer.Position - manifestOffset);

                uint iconOffset = (uint)writer.Position;
                if (ModDetails.Icon != null)
                    writer.Write(((MemoryStream)(((BitmapImage)ModDetails.Icon).StreamSource)).ToArray());
                int iconSize = (int)(writer.Position - iconOffset);

                uint screenShotsOffset = (uint)writer.Position;
                writer.Write(ModDetails.Screenshots.Count);
                for (int i = 0; i < ModDetails.Screenshots.Count; i++)
                {
                    byte[] buffer = ((MemoryStream)(((BitmapImage)ModDetails.Screenshots[i]).StreamSource)).ToArray();
                    writer.Write(buffer.Length);
                    writer.Write(buffer);
                }

                writer.Position = 0x08;
                writer.Write(manifestOffset);
                writer.Write(manifestSize);
                writer.Write(iconOffset);
                writer.Write(iconSize);
                writer.Write(screenShotsOffset);

                return writer.ToByteArray();
            }
        }
    }
}
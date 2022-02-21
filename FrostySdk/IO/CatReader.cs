using System.IO;
using FrostySdk.Interfaces;

namespace FrostySdk.IO
{
    public struct CatResourceEntry
    {
        public Sha1 Sha1;
        public uint Offset;
        public uint Size;
        public uint LogicalOffset;
        public int ArchiveIndex;

        public bool IsEncrypted;
        public uint Unknown;
        public string KeyId;
        public byte[] UnknownData;
        public uint EncryptedSize;
    }

    public struct CatPatchEntry
    {
        public Sha1 Sha1;
        public Sha1 BaseSha1;
        public Sha1 DeltaSha1;
    }

    public class CatReader : NativeReader
    {
        public const string CatMagic = "NyanNyanNyanNyan";

        public uint ResourceCount { get; }
        public uint PatchCount { get; }
        public uint EncryptedCount { get; }

        public CatReader(Stream inStream, IDeobfuscator inDeobfuscator)
            : base(inStream, inDeobfuscator)
        {
            string magic = ReadSizedString(16);
            if (magic != CatMagic)
                return;

            ResourceCount = (uint)(Length - Position) / 0x20;
            PatchCount = 0;
            EncryptedCount = 0;

            if (ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedRivals && ProfilesLibrary.DataVersion != (int)ProfileVersion.DragonAgeInquisition && ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield4 && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeed)
            {
                ResourceCount = ReadUInt();
                PatchCount = ReadUInt();

                if (ProfilesLibrary.DataVersion == (int)ProfileVersion.MassEffectAndromeda || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa17 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.Fifa18 
                    || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                {
                    EncryptedCount = ReadUInt();
                    Position += 12;

                    if (ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsBattlefrontII || ProfilesLibrary.DataVersion == (int)ProfileVersion.NeedForSpeedPayback || ProfilesLibrary.DataVersion == (int)ProfileVersion.Madden19 || ProfilesLibrary.DataVersion == (int)ProfileVersion.Battlefield5 || ProfilesLibrary.DataVersion == (int)ProfileVersion.StarWarsSquadrons)
                        EncryptedCount = 0;
                }
            }
        }

        public CatResourceEntry ReadResourceEntry()
        {
            CatResourceEntry entry = new CatResourceEntry
            {
                Sha1 = ReadSha1(),
                Offset = ReadUInt(),
                Size = ReadUInt()
            };

            if (ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeedRivals && ProfilesLibrary.DataVersion != (int)ProfileVersion.DragonAgeInquisition && ProfilesLibrary.DataVersion != (int)ProfileVersion.Battlefield4 && ProfilesLibrary.DataVersion != (int)ProfileVersion.NeedForSpeed)
                entry.LogicalOffset = ReadUInt();
            entry.ArchiveIndex = ReadInt() & 0xFF;
            return entry;
        }

        public CatResourceEntry ReadEncryptedEntry()
        {
            CatResourceEntry entry = new CatResourceEntry
            {
                Sha1 = ReadSha1(),
                Offset = ReadUInt(),
                Size = ReadUInt(),
                LogicalOffset = ReadUInt(),
                ArchiveIndex = ReadInt() & 0xFF,
                Unknown = ReadUInt(),
                IsEncrypted = true,
                KeyId = ReadSizedString(8),
                UnknownData = ReadBytes(32)
            };

            entry.EncryptedSize = entry.Size;
            entry.Size = entry.Size + (16 - (entry.Size % 16));

            return entry;
        }

        public CatPatchEntry ReadPatchEntry()
        {
            CatPatchEntry entry = new CatPatchEntry
            {
                Sha1 = ReadSha1(),
                BaseSha1 = ReadSha1(),
                DeltaSha1 = ReadSha1()
            };
            return entry;
        }
    }
}

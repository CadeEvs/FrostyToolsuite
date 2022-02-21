using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System.IO;

namespace IesResourcePlugin
{
    public class IesResource : Resource
    {
        public int Size => size;
        public Stream Data => data;

        private int size;
        private float unknown1;
        private float unknown2;
        private MemoryStream data;

        public IesResource()
        {
        }

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            base.Read(reader, am, entry, modifiedData);
            size = reader.ReadInt();
            unknown1 = reader.ReadFloat();
            unknown2 = reader.ReadFloat();
            long dataOffset = reader.ReadInt();

            reader.Position += 0x10;

            data = new MemoryStream(reader.ReadBytes(size * size * 2));

            // relocation table
            reader.ReadInt(); // dataOffset
        }
    }
}

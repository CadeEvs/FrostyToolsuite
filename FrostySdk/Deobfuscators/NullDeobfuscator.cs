using FrostySdk.Interfaces;
using FrostySdk.IO;

namespace FrostySdk.Deobfuscators
{
    public class NullDeobfuscator : IDeobfuscator
    {
        public long Initialize(NativeReader reader)
        {
            uint magic = reader.ReadUInt();
            if (magic != 0x01CED100 && magic != 0x03CED100)
            {
                reader.Position = 0;
                return -1;
            }
            reader.Position = 0x22C;
            return reader.Length;
        }

        public bool AdjustPosition(NativeReader reader, long newPosition) => false;

        public void Deobfuscate(byte[] buffer, long position, int offset, int numBytes)
        {
        }
    }
}

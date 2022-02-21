using FrostySdk.Interfaces;
using FrostySdk.IO;

namespace FrostySdk.Deobfuscators
{
    public class DADeobfuscator : IDeobfuscator
    {
        private byte[] key;
        public long Initialize(NativeReader reader)
        {
            uint magic = reader.ReadUInt();
            if (magic != 0x01CED100 && magic != 0x03CED100)
            {
                reader.Position = 0;
                return -1;
            }
            
            if (magic == 0x01CED100)
            {
                reader.Position = 0x128;
                key = reader.ReadBytes(260);

                for (int i = 0; i < key.Length; i++)
                    key[i] ^= 123;
            }

            reader.Position = 0x22C;
            return reader.Length;
        }

        public bool AdjustPosition(long newPosition) => false;

        public void Deobfuscate(byte[] buffer, long position, int offset, int numBytes)
        {
            if (key == null)
                return;

            long startPos = (position - numBytes) - 0x22C;
            for (int i = 0; i < numBytes; i++)
                buffer[i] = (byte)(key[(startPos + i) % 257] ^ buffer[i]);
        }

        public bool AdjustPosition(NativeReader reader, long newPosition) => false;
    }
}

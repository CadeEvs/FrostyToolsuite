using System;
using System.IO;
using FrostySdk.Interfaces;
using FrostySdk.IO;

namespace FrostySdk.Deobfuscators
{
    public class MEADeobfuscator : IDeobfuscator
    {
        private uint magic;
        private byte obfuscationType;
        private byte initialValue;
        private byte currentValue;

        public long Initialize(NativeReader reader)
        {
            if (reader.Length < 36)
                return -1;

            reader.Position = reader.Length - 36;

            int obfuscationDataSize = reader.ReadInt();
            string obfuscationKey = reader.ReadSizedString(32);

            long retLength = reader.Length;
            if (obfuscationKey == "@e!adnXd$^!rfOsrDyIrI!xVgHeA!6Vc")
            {
                reader.Position = reader.Length - obfuscationDataSize;
                byte[] buffer = reader.ReadBytes(obfuscationDataSize);

                uint tmpA = 0;
                uint tmpB = 0;
                uint subTotalA = 0;
                uint subTotalB = 0;
                uint total = 0;

                short unknown = BitConverter.ToInt16(buffer, 392);
                if (unknown != 0)
                {
                    for (int i = 0; i < unknown; i++)
                    {
                        byte c = buffer[410 + i];
                        tmpA = c ^ 2 * (c + tmpA);
                    }
                }

                tmpB += (uint)(buffer[405] ^ 2 * (buffer[405] + (buffer[404] ^ 2 * (buffer[404] + (buffer[403] ^ 2 * (buffer[403] + (buffer[402] ^ 2 * buffer[402])))))));
                tmpB += (uint)((buffer[3] ^ 2 * (buffer[3] + (buffer[2] ^ 2 * (buffer[2] + (buffer[1] ^ 2 * (buffer[1] + (buffer[0] ^ 2 * buffer[0]))))))));
                tmpB += (uint)(buffer[391] ^ 2 * buffer[391]);
                tmpB += tmpA;
                tmpB += (uint)(buffer[397] ^ 2 * (buffer[397] + (buffer[396] ^ 2 * (buffer[396] + (buffer[395] ^ 2 * (buffer[395] + (buffer[394] ^ 2 * buffer[394])))))));

                subTotalA += (uint)(buffer[409] ^ 2 * (buffer[409] + (buffer[408] ^ 2 * (buffer[408] + (buffer[407] ^ 2 * (buffer[407] + (buffer[406] ^ 2 * buffer[406])))))));
                subTotalA += tmpB;

                for (int i = 0; i < 129; i++)
                {
                    byte x = buffer[((i * 3) + 5) - 1];
                    byte y = buffer[((i * 3) + 5)];
                    byte z = buffer[((i * 3) + 5) + 1];
                    subTotalB = z ^ 2 * (z + (y ^ 2 * (y + (x ^ 2 * (x + subTotalB)))));
                }

                total = subTotalB + subTotalA;

                if (unknown != 0)
                    DeobfuscateBlock(buffer, 410, unknown);

                DeobfuscateBlock(buffer, 394, 4);
                DeobfuscateBlock(buffer, 0, 4);
                DeobfuscateBlock(buffer, 402, 4);
                DeobfuscateBlock(buffer, 406, 4);
                DeobfuscateBlock(buffer, 4, 387);

                magic = BitConverter.ToUInt32(buffer, 0);
                obfuscationType = buffer[4];
                initialValue = (byte)(buffer[5] ^ total);
                currentValue = initialValue;

                retLength -= obfuscationDataSize;
            }

            reader.Position = 0;

            uint tmpMagic = reader.ReadUInt();
            if (tmpMagic == 0x01CED100 || tmpMagic == 0x03CED100)
                reader.Position = 0x22C;
            else
                reader.Position = 0;

            return retLength;
        }

        public bool AdjustPosition(NativeReader reader, long newPosition)
        {
            if (magic == 0)
                return false;

            if (newPosition == 0)
            {
                currentValue = initialValue;
                return false;
            }

            if (reader.Position > newPosition)
                throw new InvalidOperationException("Cannot move backwards in obfuscated stream");

            while (reader.Position != newPosition)
                reader.ReadByte();

            return true;
        }

        public void Deobfuscate(byte[] buffer, long position, int offset, int numBytes)
        {
            if (magic != 4)
                return;

            long startPos = position - numBytes;
            if (obfuscationType == 2)
            {
                for (int i = 0; i < numBytes; i++)
                {
                    byte b = buffer[i];
                    buffer[i] = (byte)(currentValue ^ b);
                    currentValue = (byte)((b ^ initialValue) - (startPos + i));
                }
            }
            else
                throw new InvalidDataException("Unimplemented obfuscation method used");
        }

        private static void DeobfuscateBlock(byte[] buffer, int offset, int count)
        {
            int a = 1172968056;
            int z = 0;

            for (int i = 0; i < count; i++)
            {
                int b = (byte)(buffer[i + offset] ^ (a + ((a >> 8) & 0xFF) + (a >> 16) + ((a >> 24) & 0xFF)));
                buffer[i + offset] = (byte)b;

                int c = RollOver(a, b & 0x1F);
                a = RollOver((b | ((b | ((b | (b << 8)) << 8)) << 8)) + c, 1);

                if (z > 16)
                {
                    a *= 2;
                    z = 0;
                }
                z++;
            }
        }

        private static int RollOver(int value, int count)
        {
            const int nbits = sizeof(uint) * 8;
            count %= nbits;
            int high = value >> (nbits - count);
            high &= ~(-1 << count);
            value <<= count;
            value |= high;
            return value;
        }
    }
}

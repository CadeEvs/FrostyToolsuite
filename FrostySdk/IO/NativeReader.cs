using System;
using System.IO;
using FrostySdk.Interfaces;
using System.Text;

namespace FrostySdk.IO
{
    public enum Endian
    {
        Little,
        Big
    }

    public class NativeReader : IDisposable
    {
        public Stream BaseStream => stream;

        public virtual long Position
        {
            get => stream?.Position ?? 0;
            set
            {
                if (deobfuscator == null || !deobfuscator.AdjustPosition(this, value))
                    stream.Position = value;
            }
        }
        public virtual long Length => streamLength;

        protected Stream stream;
        protected IDeobfuscator deobfuscator;
        protected byte[] buffer;
        protected char[] charBuffer;
        protected long streamLength;
        protected Encoding wideDecoder;

        public NativeReader(Stream inStream)
        {
            stream = inStream;
            if (stream != null)
                streamLength = stream.Length;

            wideDecoder = new UnicodeEncoding();
            buffer = new byte[20];
            charBuffer = new char[2];
        }

        public NativeReader(Stream inStream, IDeobfuscator inDeobfuscator)
            : this(inStream)
        {
            deobfuscator = inDeobfuscator;
            if (deobfuscator != null && stream != null)
            {
                long newLength = deobfuscator.Initialize(this);
                if (newLength != -1)
                    streamLength = newLength;
            }
        }

        public static byte[] ReadInStream(Stream inStream)
        {
            using (NativeReader reader = new NativeReader(inStream))
                return reader.ReadToEnd();
        }

        #region -- Basic Types --

        public char ReadWideChar()
        {
            FillBuffer(2);
            wideDecoder.GetChars(buffer, 0, 2, charBuffer, 0);
            return charBuffer[0];
        }

        public bool ReadBoolean() => ReadByte() == 1;

        public byte ReadByte()
        {
            FillBuffer(1);
            return buffer[0];
        }

        public sbyte ReadSByte()
        {
            FillBuffer(1);
            return (sbyte)buffer[0];
        }

        public short ReadShort(Endian inEndian = Endian.Little)
        {
            FillBuffer(2);
            if (inEndian == Endian.Little)
                return (short)(buffer[0] | buffer[1] << 8);
            return (short)(buffer[1] | buffer[0] << 8);
        }

        public ushort ReadUShort(Endian inEndian = Endian.Little)
        {
            FillBuffer(2);
            if (inEndian == Endian.Little)
                return (ushort)(buffer[0] | buffer[1] << 8);
            return (ushort)(buffer[1] | buffer[0] << 8);
        }

        public int ReadInt(Endian inEndian = Endian.Little)
        {
            FillBuffer(4);
            if (inEndian == Endian.Little)
                return (int)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
            return (int)(buffer[3] | buffer[2] << 8 | buffer[1] << 16 | buffer[0] << 24);
        }

        public uint ReadUInt(Endian inEndian = Endian.Little)
        {
            FillBuffer(4);
            if (inEndian == Endian.Little)
                return (uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
            return (uint)(buffer[3] | buffer[2] << 8 | buffer[1] << 16 | buffer[0] << 24);
        }

        public long ReadLong(Endian inEndian = Endian.Little)
        {
            FillBuffer(8);
            if (inEndian == Endian.Little)
                return (long)(uint)(buffer[4] | buffer[5] << 8 | buffer[6] << 16 | buffer[7] << 24) << 32 |
                       (long)(uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
            return (long)(uint)(buffer[3] | buffer[2] << 8 | buffer[1] << 16 | buffer[0] << 24) << 32 |
                   (long)(uint)(buffer[7] | buffer[6] << 8 | buffer[5] << 16 | buffer[4] << 24);
        }

        public ulong ReadULong(Endian inEndian = Endian.Little)
        {
            FillBuffer(8);
            if (inEndian == Endian.Little)
                return (ulong)(uint)(buffer[4] | buffer[5] << 8 | buffer[6] << 16 | buffer[7] << 24) << 32 |
                       (ulong)(uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
            return (ulong)(uint)(buffer[3] | buffer[2] << 8 | buffer[1] << 16 | buffer[0] << 24) << 32 |
                   (ulong)(uint)(buffer[7] | buffer[6] << 8 | buffer[5] << 16 | buffer[4] << 24);
        }

        public unsafe float ReadFloat(Endian inEndian = Endian.Little)
        {
            FillBuffer(4);

            uint tmpBuffer = 0;
            if (inEndian == Endian.Little)
                tmpBuffer = (uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
            else
                tmpBuffer = (uint)(buffer[3] | buffer[2] << 8 | buffer[1] << 16 | buffer[0] << 24);

            return *((float*)&tmpBuffer);
        }

        public unsafe double ReadDouble(Endian inEndian = Endian.Little)
        {
            FillBuffer(8);

            uint lo = 0;
            uint hi = 0;

            if (inEndian == Endian.Little)
            {
                lo = (uint)(buffer[0] | buffer[1] << 8 | buffer[2] << 16 | buffer[3] << 24);
                hi = (uint)(buffer[4] | buffer[5] << 8 | buffer[6] << 16 | buffer[7] << 24);
            }
            else
            {
                lo = (uint)(buffer[3] | buffer[2] << 8 | buffer[1] << 16 | buffer[0] << 24);
                hi = (uint)(buffer[7] | buffer[6] << 8 | buffer[5] << 16 | buffer[4] << 24);
            }

            ulong tmpBuffer = ((ulong)hi) << 32 | lo;
            return *((double*)&tmpBuffer);
        }

        #endregion

        #region -- Special Types --

        public Guid ReadGuid(Endian endian = Endian.Little)
        {
            FillBuffer(16);
            if (endian == Endian.Little)
                return new Guid(new byte[] {
                        buffer[0], buffer[1], buffer[2], buffer[3], buffer[4], buffer[5], buffer[6], buffer[7],
                        buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15]
                    });

            return new Guid(new byte[] {
                    buffer[3], buffer[2], buffer[1], buffer[0], buffer[5], buffer[4], buffer[7], buffer[6],
                    buffer[8], buffer[9], buffer[10], buffer[11], buffer[12], buffer[13], buffer[14], buffer[15]
                });
        }

        public Sha1 ReadSha1()
        {
            FillBuffer(20);
            return new Sha1(buffer);
        }

        public int Read7BitEncodedInt()
        {
            int result = 0;
            int i = 0;

            while (true)
            {
                int b = ReadByte();
                result |= (b & 127) << i;

                if (b >> 7 == 0)
                    return result;

                i += 7;
            }
        }

        public long Read7BitEncodedLong()
        {
            long result = 0;
            int i = 0;

            while (true)
            {
                int b = ReadByte();
                result |= (long)((b & 127) << i);

                if (b >> 7 == 0)
                    return result;

                i += 7;
            }
        }

        #endregion

        #region -- String Types --

        public string ReadNullTerminatedString()
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                char c = (char)ReadByte();
                if (c == 0x00)
                    return sb.ToString();

                sb.Append(c);
            }
        }

        public string ReadNullTerminatedWideString()
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                char c = ReadWideChar();
                if (c == 0x0000)
                    return sb.ToString();

                sb.Append(c);
            }
        }

        public string ReadSizedString(int strLen)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strLen; i++)
            {
                char c = (char)ReadByte();
                if (c != 0x00)
                    sb.Append(c);
            }
            return sb.ToString();
        }

        public string ReadLine()
        {
            StringBuilder sb = new StringBuilder();
            byte c = 0x00;

            while (c != 0x0d && c != 0x0a)
            {
                c = ReadByte();
                sb.Append((char)c);
                if (c == 0x0a || c == 0x0d || Position >= Length)
                    break;
            }

            if (c == 0x0d)
                ReadByte();

            return sb.ToString().Trim('\r', '\n');
        }

        public string ReadWideLine()
        {
            StringBuilder sb = new StringBuilder();
            char c = (char)0x00;

            while (c != 0x0d && c != 0x0a)
            {
                c = ReadWideChar();
                sb.Append(c);
                if (c == 0x0a || c == 0x0d || Position >= Length)
                    break;
            }

            if (c == 0x0d)
                ReadWideChar();

            return sb.ToString().Trim('\r', '\n');
        }

        public void Pad(int alignment)
        {
            if (alignment == 0)
            {
                return;
            }

            while (Position % alignment != 0)
            {
                Position++;
            }
        }

        #endregion

        public byte[] ReadToEnd()
        {
            long totalSize = Length - Position;
            if (totalSize < int.MaxValue)
                return ReadBytes((int)totalSize);

            byte[] outBuffer = new byte[totalSize];
            while(totalSize > 0)
            {
                int bufferSize = (totalSize > int.MaxValue) ? int.MaxValue : (int)totalSize;
                byte[] tmpBuffer = new byte[bufferSize];

                int count = Read(tmpBuffer, 0, bufferSize);
                totalSize -= bufferSize;

                Buffer.BlockCopy(tmpBuffer, 0, outBuffer, count, bufferSize);
            }

            return outBuffer;
        }

        public byte[] ReadBytes(int count)
        {
            byte[] outBuffer = new byte[count];
            int totalNumBytesRead = 0;

            do
            {
                int numBytesRead = Read(outBuffer, totalNumBytesRead, count);
                if (numBytesRead == 0)
                    break;

                totalNumBytesRead += numBytesRead;
                count -= numBytesRead;

            } while (count > 0);

            return outBuffer;
        }

        public virtual int Read(byte[] inBuffer, int offset, int numBytes)
        {
            int count = stream.Read(inBuffer, offset, numBytes);
            deobfuscator?.Deobfuscate(inBuffer, Position, offset, numBytes);
            return count;
        }

        public Stream CreateViewStream(long offset, long size)
        {
            Position = offset;
            return new MemoryStream(ReadBytes((int)size));
        }

        public void Dispose() => Dispose(true);

        protected virtual void FillBuffer(int numBytes)
        {
            stream.Read(buffer, 0, numBytes);
            deobfuscator?.Deobfuscate(buffer, Position, 0, numBytes);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stream copyOfStream = stream;
                stream = null;

                copyOfStream?.Close();
            }

            stream = null;
            buffer = null;
        }
    }

}

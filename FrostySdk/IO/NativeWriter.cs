using System;
using System.IO;
using System.Text;

namespace FrostySdk.IO
{
    public class NativeWriter : BinaryWriter
    {
        public long Position { get => BaseStream.Position; set => BaseStream.Position = value; }
        public long Length => BaseStream.Length;

        public NativeWriter(Stream inStream, bool leaveOpen = false, bool wide = false)
            : base(inStream, (wide) ? Encoding.Unicode : Encoding.Default, leaveOpen)
        {
        }

        public void Write(Guid value, Endian endian)
        {
            if (endian == Endian.Big)
            {
                byte[] b = value.ToByteArray();
                Write(b[3]); Write(b[2]); Write(b[1]); Write(b[0]);
                Write(b[5]); Write(b[4]);
                Write(b[7]); Write(b[6]);
                for (int i = 0; i < 8; i++)
                    Write(b[8 + i]);
            }
            else
                Write(value);
        }

        public void Write(short value, Endian endian)
        {
            if (endian == Endian.Big)
                Write((short)((ushort)(((value & 0xFF) << 8) | ((value & 0xFF00) >> 8))));
            else
                Write(value);
        }

        public void Write(ushort value, Endian endian)
        {
            if (endian == Endian.Big)
                Write(((ushort)(((value & 0xFF) << 8) | ((value & 0xFF00) >> 8))));
            else
                Write(value);
        }

        public void Write(int value, Endian endian)
        {
            if (endian == Endian.Big)
                Write(((value & 0xFF) << 24) | ((value & 0xFF00) << 8) | ((value >> 8) & 0xFF00) | ((value >> 24) & 0xFF));
            else
                Write(value);
        }

        public void Write(uint value, Endian endian)
        {
            if (endian == Endian.Big)
                Write(((value & 0xFF) << 24) | ((value & 0xFF00) << 8) | ((value >> 8) & 0xFF00) | ((value >> 24) & 0xFF));
            else
                Write(value);
        }

        public void Write(long value, Endian endian)
        {
            if (endian == Endian.Big)
            {
                Write(((long)((value & 0xFF) << 56) | ((value & 0xFF00) << 40) | ((value & 0xFF0000) << 24) | ((value & 0xFF000000) << 8))
                    | ((long)((value >> 8) & 0xFF000000) | ((value >> 24) & 0xFF0000) | ((value >> 40) & 0xFF00) | ((value >> 56) & 0xFF)));
            }
            else
                Write(value);
        }

        public void Write(ulong value, Endian endian)
        {
            if (endian == Endian.Big)
            {
                Write(((ulong)((value & 0xFF) << 56) | ((value & 0xFF00) << 40) | ((value & 0xFF0000) << 24) | ((value & 0xFF000000) << 8))
                    | ((ulong)((value >> 8) & 0xFF000000) | ((value >> 24) & 0xFF0000) | ((value >> 40) & 0xFF00) | ((value >> 56) & 0xFF)));
            }
            else
                Write(value);
        }

        private void WriteString(string str)
        {
            for (int i = 0; i < str.Length; i++)
                Write(str[i]);
        }

        public void WriteNullTerminatedString(string str)
        {
            WriteString(str);
            Write((char)0x00);
        }

        public void WriteSizedString(string str)
        {
            Write7BitEncodedInt(str.Length);
            WriteString(str);
        }

        public void WriteFixedSizedString(string str, int size)
        {
            WriteString(str);
            for (int i = 0; i < (size - str.Length); i++)
                Write((char)0x00);
        }

        public new void Write7BitEncodedInt(int value)
        {
            uint v = (uint)value;
            while (v >= 0x80)
            {
                Write((byte)(v | 0x80));
                v >>= 7;
            }
            Write((byte)v);
        }

        public void Write7BitEncodedLong(long value)
        {
            ulong v = (ulong)value;
            while (v >= 0x80)
            {
                Write((byte)(v | 0x80));
                v >>= 7;
            }
            Write((byte)v);
        }

        public void Write(Guid value) => Write(value.ToByteArray(), 0, 16);

        public void Write(Sha1 value) => Write(value.ToByteArray(), 0, 20);

        public void WriteLine(string str)
        {
            WriteString(str);
            Write((char)0x0D);
            Write((char)0x0A);
        }

        public void WritePadding(byte alignment)
        {
            while (Position % alignment != 0)
                Write((byte)0x00);
        }

        public byte[] ToByteArray() => BaseStream is MemoryStream stream ? stream.ToArray() : null;
    }
}

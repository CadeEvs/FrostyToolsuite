using System;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Frosty.Sdk.IO;

public unsafe class DataStream : IDisposable
{
    public long Position
    {
        get => m_stream.Position;
        set => m_stream.Position = value;
    }

    public long Length => m_stream.Length;
    
    protected Stream m_stream;
    private readonly byte[] m_buffer;
    private readonly StringBuilder m_stringBuilder;

    protected DataStream()
    {
        m_stream = Stream.Null;
        m_buffer = new byte[20];
        m_stringBuilder = new StringBuilder();
    }
    
    public DataStream(Stream inStream)
    {
        m_stream = inStream;
        m_buffer = new byte[20];
        m_stringBuilder = new StringBuilder();
    }
    
    public long Seek(long offset, SeekOrigin origin) => m_stream.Seek(offset, origin);

    public void CopyTo(Stream destination) => m_stream.CopyTo(destination);
    public void CopyTo(Stream destination, int length) => m_stream.CopyTo(destination, length);

    public void SetLength(int value) => m_stream.SetLength(value);

    #region -- Read --

    public byte[] ReadBytes(int count)
    {
        byte[] retVal = new byte[count];
        m_stream.ReadExactly(retVal, 0, retVal.Length);
        return retVal;
    }
    public void ReadExactly(Span<byte> buffer) => m_stream.ReadExactly(buffer);
    
    public int Read(byte[] buffer, int offset, int count) => m_stream.Read(buffer, offset, count);
    
    #region -- Basic Types --

    public byte ReadByte()
    {
        int retVal = m_stream.ReadByte();
        if (retVal == -1)
        {
            throw new EndOfStreamException();
        }

        return (byte)retVal;
    }
    
    public sbyte ReadSByte()
    {
        return (sbyte)ReadByte();
    }

    public bool ReadBoolean()
    {
        return ReadByte() != 0;
    }

    public char ReadChar(bool wide = false)
    {
        return (char)(wide ? ReadInt16() : ReadByte());
    }

    public short ReadInt16(Endian endian = Endian.Little)
    {
        m_stream.ReadExactly(m_buffer, 0, sizeof(short));
        
        if (endian == Endian.Big)
        {
            Array.Reverse(m_buffer, 0, sizeof(short));
        }
        return BitConverter.ToInt16(m_buffer);
    }

    public ushort ReadUInt16(Endian endian = Endian.Little)
    {
        return (ushort)ReadInt16(endian);
    }
    
    public int ReadInt32(Endian endian = Endian.Little)
    {
        Span<byte> span = m_buffer.AsSpan(0, sizeof(int));
        m_stream.ReadExactly(span);
        
        if (endian == Endian.Big)
        {
            return BinaryPrimitives.ReadInt32BigEndian(span);
        }

        return BinaryPrimitives.ReadInt32LittleEndian(span);
    }
    
    public uint ReadUInt32(Endian endian = Endian.Little)
    {
        return (uint)ReadInt32(endian);
    }
    
    public long ReadInt64(Endian endian = Endian.Little)
    {
        Span<byte> span = m_buffer.AsSpan(0, sizeof(long));
        m_stream.ReadExactly(span);
        
        if (endian == Endian.Big)
        {
            return BinaryPrimitives.ReadInt64BigEndian(span);
        }

        return BinaryPrimitives.ReadInt64LittleEndian(span);
    }

    public ulong ReadUInt64(Endian endian = Endian.Little)
    {
        return (ulong)ReadInt64(endian);
    }
    
    public float ReadSingle(Endian endian = Endian.Little)
    {
        Span<byte> span = m_buffer.AsSpan(0, sizeof(float));
        m_stream.ReadExactly(span);
        
        if (endian == Endian.Big)
        {
            return BinaryPrimitives.ReadSingleBigEndian(span);
        }

        return BinaryPrimitives.ReadSingleLittleEndian(span);
    }
    
    public double ReadDouble(Endian endian = Endian.Little)
    {
        Span<byte> span = m_buffer.AsSpan(0, sizeof(double));
        m_stream.ReadExactly(span);
        
        if (endian == Endian.Big)
        {
            return BinaryPrimitives.ReadDoubleBigEndian(span);
        }

        return BinaryPrimitives.ReadDoubleLittleEndian(span);
    }

    #endregion

    #region -- Strings --
    
    public string ReadNullTerminatedString(bool wide = false)
    {
        m_stringBuilder.Clear();
        while (true)
        {
            char c = ReadChar(wide);
            if (c == 0)
            {
                return m_stringBuilder.ToString();
            }

            m_stringBuilder.Append(c);
        }
    }

    public string ReadFixedSizedString(int size)
    {
        Span<byte> buffer = new byte[size];
        m_stream.ReadExactly(buffer);

        return Encoding.UTF8.GetString(buffer).TrimEnd((char)0);
    }

    public string ReadSizedString()
    {
        return ReadFixedSizedString(Read7BitEncodedInt32());
    }

    #endregion

    public int Read7BitEncodedInt32()
    {
        uint num1 = 0;
        for (int index = 0; index < 28; index += 7)
        {
            byte num2 = ReadByte();
            num1 |= (uint) ((num2 & sbyte.MaxValue) << index);
            if (num2 <= 127)
            {
                return (int) num1;
            }
        }
        byte num3 = ReadByte();
        if (num3 > 15)
        {
            throw new FormatException();
        }

        return (int) (num1 | (uint) num3 << 28);
    }

    public long Read7BitEncodedInt64()
    {
        ulong num1 = 0;
        for (int index = 0; index < 63; index += 7)
        {
            byte num2 = ReadByte();
            num1 |= (ulong) (((long) num2 & sbyte.MaxValue) << index);
            if (num2 <= 127)
            {
                return (long) num1;
            }
        }
        byte num3 = ReadByte();
        if (num3 > 1)
        {
            throw new FormatException();
        }

        return (long) (num1 | (ulong) num3 << 63);
    }

    public Guid ReadGuid(Endian endian = Endian.Little)
    {
        Span<byte> span = m_buffer.AsSpan(0, sizeof(Guid));
        m_stream.ReadExactly(span);

        if (endian == Endian.Big)
        {
            return new Guid(BinaryPrimitives.ReadInt32BigEndian(span),
                BinaryPrimitives.ReadInt16BigEndian(span[4..]), BinaryPrimitives.ReadInt16BigEndian(span[6..]),
                span[8..].ToArray());
        }

        return new Guid(span);
    }

    public Sha1 ReadSha1()
    {
        Span<byte> span = m_buffer.AsSpan(0, sizeof(Sha1));
        m_stream.ReadExactly(span);
        
        return new Sha1(span);
    }

    #endregion
    
    #region -- Write --

    public void Write(ReadOnlySpan<byte> buffer) => m_stream.Write(buffer);

    public void Write(byte[] buffer, int offset, int count) => m_stream.Write(buffer, offset, count);

    #region -- Basic Types --

    public void WriteByte(byte value)
    {
        m_stream.WriteByte(value);
    }

    public void WriteSByte(sbyte value)
    {
        WriteByte((byte)value);
    }

    public void WriteChar(char value, bool wide = false)
    {
        if (wide)
        {
            WriteInt16((short)value);
        }
        else
        {
            WriteByte((byte)value);
        }
    }

    public void WriteInt16(short value, Endian endian = Endian.Little)
    {
        if (endian == Endian.Big)
        {
            value = Reverse(value);
        }
        Unsafe.As<byte, short>(ref m_buffer[0]) = value;
        m_stream.Write(m_buffer, 0, sizeof(short));
    }

    public void WriteUInt16(ushort value, Endian endian = Endian.Little)
    {
        WriteInt16((short)value, endian);
    }
    
    public void WriteInt32(int value, Endian endian = Endian.Little)
    {
        if (endian == Endian.Big)
        {
            value = Reverse(value);
        }
        Unsafe.As<byte, int>(ref m_buffer[0]) = value;
        m_stream.Write(m_buffer, 0, sizeof(int));
    }
    
    public void WriteUInt32(uint value, Endian endian = Endian.Little)
    {
        WriteInt32((int)value, endian);
    }
    
    public void WriteInt64(long value, Endian endian = Endian.Little)
    {
        if (endian == Endian.Big)
        {
            value = Reverse(value);
        }
        Unsafe.As<byte, long>(ref m_buffer[0]) = value;
        m_stream.Write(m_buffer, 0, sizeof(long));
    }

    public void WriteUInt64(ulong value, Endian endian = Endian.Little)
    {
        WriteInt64((long)value, endian);
    }

    public void WriteSingle(float value, Endian endian = Endian.Little)
    {
        if (endian == Endian.Big)
        {
            int i = 0;
            Unsafe.As<int, float>(ref i) = value;
            i = Reverse(i);
            Unsafe.As<byte, int>(ref m_buffer[0]) = i;
        }
        else
        {
            Unsafe.As<byte, float>(ref m_buffer[0]) = value;   
        }
        m_stream.Write(m_buffer, 0, sizeof(float));
    }

    public void WriteDouble(double value, Endian endian = Endian.Little)
    {
        if (endian == Endian.Big)
        {
            long i = 0;
            Unsafe.As<long, double>(ref i) = value;
            i = Reverse(i);
            Unsafe.As<byte, long>(ref m_buffer[0]) = i;
        }
        else
        {
            Unsafe.As<byte, double>(ref m_buffer[0]) = value;   
        }
        m_stream.Write(m_buffer, 0, sizeof(double));
    }

    #endregion

    #region -- Strings --

    public void WriteNullTerminatedString(string value, bool wide = false)
    {
        Span<byte> span = new byte[(value.Length + 1) * (wide ? 2 : 1)];
        if (wide)
        {
            Encoding.Unicode.GetBytes(value, span);
        }
        else
        {
            Encoding.ASCII.GetBytes(value, span);
        }
        m_stream.Write(span);
    }

    public void WriteFixedSizedString(string value, int size)
    {
        Span<byte> span = new byte[size];
        Encoding.UTF8.GetBytes(value, span);
        m_stream.Write(span);
    }
    
    public void WriteSizedString(string value)
    {
        int size = Encoding.UTF8.GetByteCount(value) + 1;
        Write7BitEncodedInt32(size);
        WriteFixedSizedString(value, size);
    }

    #endregion

    public void WriteGuid(Guid value, Endian endian = Endian.Little)
    {
        Unsafe.As<byte, Guid>(ref m_buffer[0]) = value;
        
        if (endian == Endian.Big)
        {
            Array.Reverse(m_buffer, 0, sizeof(Guid));
        }
        m_stream.Write(m_buffer, 0, sizeof(Guid));
    }

    public void WriteSha1(Sha1 value, Endian endian = Endian.Little)
    {
        Unsafe.As<byte, Sha1>(ref m_buffer[0]) = value;
        
        if (endian == Endian.Big)
        {
            Array.Reverse(m_buffer, 0, sizeof(Sha1));
        }
        m_stream.Write(m_buffer, 0, sizeof(Sha1));
    }
    
    public void Write7BitEncodedInt32(int value)
    {
        uint num;
        for (num = (uint) value; num > (uint) sbyte.MaxValue; num >>= 7)
        {
            WriteByte((byte) (num | 0xFFFFFF80U));
        }

        WriteByte((byte) num);
    }

    public void Write7BitEncodedInt64(long value)
    {
        ulong num;
        for (num = (ulong) value; num > (ulong) sbyte.MaxValue; num >>= 7)
        {
            WriteByte((byte) ((uint) num | 0xFFFFFF80U));
        }

        WriteByte((byte) num);
    }

    #endregion

    public void Pad(int alignment)
    {
        if (m_stream.Position % alignment != 0)
        {
            m_stream.Position += alignment - (m_stream.Position % alignment);
        } 
    }
    
    public static implicit operator Stream(DataStream stream) => stream.m_stream;
    
    public virtual void Dispose()
    {
        m_stream.Dispose();
    }

    private short Reverse(short s)
    {
        ushort val = (ushort)s;
        return (short)(((val & 0xff) << 8) | ((val & 0xff00) >> 8));
    }
    
    private int Reverse(int i)
    {
        uint val = (uint)i;
        return (int)(((val & 0xff) << 24) | ((val & 0xff000000) >> 24) |
                     ((val & 0xff00) << 8) | ((val & 0xff0000) >> 8));
    }

    private long Reverse(long l)
    {
        ulong val = (uint)l;
        return (long)(((val & 0x00000000000000ffUL) << 56) | ((val & 0xff00000000000000UL) >> 56) |
                      ((val & 0x000000000000ff00UL) << 40) | ((val & 0x00ff000000000000UL) >> 40) |
                      ((val & 0x0000000000ff0000UL) << 24) | ((val & 0x0000ff0000000000UL) >> 24) |
                      ((val & 0x00000000ff000000UL) <<  8) | ((val & 0x000000ff00000000UL) >> 8));
    }
}
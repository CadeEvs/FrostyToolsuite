using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Frosty.Sdk.Deobfuscators;
using Frosty.Sdk.Interfaces;
using Frosty.Sdk.Managers;

namespace Frosty.Sdk.IO;

public unsafe class DataStream : IDisposable
{
    public long Length => m_stream.Length;
    
    public long Position
    {
        get => m_stream.Position;
        set => m_stream.Position = value;
    }

    protected Stream m_stream;
    private readonly byte[] m_buffer;
    private readonly StringBuilder m_stringBuilder;

    public DataStream(Stream inStream, bool deobfuscate = false)
    {
        m_stream = inStream;
        m_buffer = new byte[20];
        m_stringBuilder = new StringBuilder(100);

        if (deobfuscate)
        {
            Stream? stream;
            if ((stream = Deobfuscator.Initialize(this)) != null)
            {
                m_stream.Dispose();
                m_stream = stream;
            }
            IDeobfuscator? deobfuscator = FileSystemManager.CreateDeobfuscator();
            if ((stream = deobfuscator?.Initialize(this)) != null)
            {
                m_stream.Dispose();
                m_stream = stream;
            }
        }
    }

    public void SetStream(Stream stream)
    {
        m_stream.Dispose();
        m_stream = stream;
    }

    public void CopyTo(Stream stream) => m_stream.CopyTo(stream);
    
    public long Seek(long offset, SeekOrigin origin) => m_stream.Seek(offset, origin);

    public void Pad(int alignment)
    {
        long r = Position % alignment;
        if (r != 0)
        {
            Position += alignment - r;
        }
    }

    #region -- Read --

    public int Read(byte[] buffer, int offset, int count) => m_stream.Read(buffer, offset, count);
    
    #region -- Basic Types --

    public byte ReadByte()
    {
        if (FillBuffer(sizeof(byte)) != sizeof(byte))
        {
            throw new EndOfStreamException();
        }

        return m_buffer[0];
    }
    
    public sbyte ReadSByte()
    {
        return (sbyte)ReadByte();
    }

    public char ReadChar(bool wide = false)
    {
        return (char)(wide ? ReadInt16() : ReadByte());
    }

    public bool ReadBoolean()
    {
        return ReadByte() != 0;
    }

    public short ReadInt16(Endian endian = Endian.Little)
    {
        if (FillBuffer(sizeof(short)) != sizeof(short))
        {
            throw new EndOfStreamException();
        }

        if (endian == Endian.Big)
        {
            return (short)(m_buffer[0] << 8 | m_buffer[1] << 0);
        }
        return (short)(m_buffer[1] << 8 | m_buffer[0] << 0);
    }

    public ushort ReadUInt16(Endian endian = Endian.Little)
    {
        return (ushort)ReadInt16(endian);
    }
    
    public int ReadInt32(Endian endian = Endian.Little)
    {
        if (FillBuffer(sizeof(int)) != sizeof(int))
        {
            throw new EndOfStreamException();
        }

        if (endian == Endian.Big)
        {
            return m_buffer[0] << 24 | m_buffer[1] << 16 | m_buffer[2] << 8 | m_buffer[3] << 0;
        }
        return m_buffer[3] << 24 | m_buffer[2] << 16 | m_buffer[1] << 8 | m_buffer[0] << 0;
    }

    public uint ReadUInt32(Endian endian = Endian.Little)
    {
        return (uint)ReadInt32(endian);
    }
    
    public long ReadInt64(Endian endian = Endian.Little)
    {
        if (FillBuffer(sizeof(long)) != sizeof(long))
        {
            throw new EndOfStreamException();
        }

        if (endian == Endian.Big)
        {
            return (long)m_buffer[0] << 56 | (long)m_buffer[1] << 48 | (long)m_buffer[2] << 40 | (long)m_buffer[3] << 32 |
                   (uint)(m_buffer[4] << 24) | (uint)(m_buffer[5] << 16) | (uint)(m_buffer[6] << 8) | (uint)(m_buffer[7] << 0);
        }
        return (long)m_buffer[7] << 56 | (long)m_buffer[6] << 48 | (long)m_buffer[5] << 40 | (long)m_buffer[4] << 32 |
               (uint)(m_buffer[3] << 24) | (uint)(m_buffer[2] << 16) | (uint)(m_buffer[1] << 8) | (uint)(m_buffer[0] << 0);
    }

    public ulong ReadUInt64(Endian endian = Endian.Little)
    {
        return (ulong)ReadInt64(endian);
    }
    
    public float ReadSingle(Endian endian = Endian.Little)
    {
        if (FillBuffer(sizeof(float)) != sizeof(float))
        {
            throw new EndOfStreamException();
        }

        int tmp;
        if (endian == Endian.Big)
        {
            tmp = (m_buffer[0] << 24 | m_buffer[1] << 16 | m_buffer[2] << 8 | m_buffer[3] << 0);
        }
        else
        {
            tmp = (m_buffer[3] << 24 | m_buffer[2] << 16 | m_buffer[1] << 8 | m_buffer[0] << 0);
        }
        
        return *(float*)&tmp;
    }
    
    public double ReadDouble(Endian endian = Endian.Little)
    {
        if (FillBuffer(sizeof(long)) != sizeof(long))
        {
            throw new EndOfStreamException();
        }

        long tmp;
        if (endian == Endian.Big)
        {
            tmp = (long)m_buffer[0] << 56 | (long)m_buffer[1] << 48 | (long)m_buffer[2] << 40 | (long)m_buffer[3] << 32 |
                  (uint)(m_buffer[4] << 24) | (uint)(m_buffer[5] << 16) | (uint)(m_buffer[6] << 8) | (uint)(m_buffer[7] << 0);
        }
        else
        {
            tmp = (long)m_buffer[7] << 56 | (long)m_buffer[6] << 48 | (long)m_buffer[5] << 40 | (long)m_buffer[4] << 32 |
                  (uint)(m_buffer[3] << 24) | (uint)(m_buffer[2] << 16) | (uint)(m_buffer[1] << 8) | (uint)(m_buffer[0] << 0);
        }
        return *(double*)&tmp;
    }

    public byte[] ReadBytes(int count)
    {
        byte[] retVal = new byte[count];
        
        if (m_stream.Read(retVal, 0, count) != count)
        {
            throw new EndOfStreamException();
        }

        return retVal;
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
                break;
            }
            m_stringBuilder.Append(c);
        }

        return m_stringBuilder.ToString();
    }

    public string ReadFixedSizedString(int size, bool wide = false)
    {
        int actualSize = size * (wide ? 2 : 1);
        m_stringBuilder.Clear();

        byte[] tmp = ReadBytes(actualSize);

        for (int i = 0; i < size; i++)
        {
            char c;
            if (wide)
            {
                c = (char)(tmp[i * 2] | tmp[i * 2 + 1] << 8);
            }
            else
            {
                c = (char)tmp[i];
            }

            if (c != 0)
            {
                m_stringBuilder.Append(c);
            }
        }

        return m_stringBuilder.ToString();
    }
    
    public string ReadSizedString()
    {
        return ReadFixedSizedString(Read7BitEncodedInt());
    }

    #endregion

    public Guid ReadGuid(Endian endian = Endian.Little)
    {
        if (FillBuffer(sizeof(Guid)) != sizeof(Guid))
        {
            throw new EndOfStreamException();
        }
        int a;
        short b, c;
        if (endian == Endian.Big)
        {
            a = m_buffer[0] << 24 | m_buffer[1] << 16 | m_buffer[2] << 8 | m_buffer[3] << 0;
            b = (short)(m_buffer[4] << 8 | m_buffer[5] << 0);
            c = (short)(m_buffer[6] << 8 | m_buffer[7] << 0);
        }
        else
        {
            a = m_buffer[3] << 24 | m_buffer[2] << 16 | m_buffer[1] << 8 | m_buffer[0] << 0; 
            b = (short)(m_buffer[5] << 8 | m_buffer[4] << 0);
            c = (short)(m_buffer[7] << 8 | m_buffer[6] << 0);
        }
        
        byte[] d = new byte[8];
        Array.Copy(m_buffer, 8, d, 0, 8);
        
        return new Guid(a, b, c, d);
    }

    public int Read7BitEncodedInt(Endian endian = Endian.Little)
    {
        int result = 0;
        int i = 0;

        while (true)
        {
            int b = ReadByte();
            result |= (b & 127) << i;

            if (b >> 7 == 0)
            {
                return result;
            }

            i += 7;
        }
    }
    
    public long Read7BitEncodedLong(Endian endian = Endian.Little)
    {
        long result = 0;
        int i = 0;

        while (true)
        {
            int b = ReadByte();
            result |= (long)((b & 127) << i);

            if (b >> 7 == 0)
            {
                return result;
            }

            i += 7;
        }
    }

    public Sha1 ReadSha1()
    {
        if (FillBuffer(sizeof(Sha1)) != sizeof(Sha1))
        {
            throw new EndOfStreamException();
        }
        
        return new Sha1(m_buffer);
    }

    #endregion
    
    #region -- Write --

    public void Write(byte[] buffer, int offset, int count) => m_stream.Write(buffer, offset, count);
    
    #region -- Basic Types --

    public void WriteByte(byte value)
    {
        m_buffer[0] = value;
        WriteBuffer(sizeof(byte));
    }

    public void WriteSByte(sbyte value)
    {
        WriteByte((byte)value);
    }

    public void WriteBoolean(bool value)
    {
        WriteByte((byte)(value ? 1 : 0));
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
        WriteToBuffer((byte*)&value, 0, sizeof(short), endian);
        
        WriteBuffer(sizeof(short));
    }

    public void WriteUInt16(ushort value, Endian endian = Endian.Little)
    {
        WriteInt16((short)value, endian);
    }
    
    public void WriteInt32(int value, Endian endian = Endian.Little)
    {
        WriteToBuffer((byte*)&value, 0, sizeof(int), endian);
        
        WriteBuffer(sizeof(int));
    }

    public void WriteUInt32(uint value, Endian endian = Endian.Little)
    {
        WriteInt32((int)value, endian);
    }
    
    public void WriteInt64(long value, Endian endian = Endian.Little)
    {
        WriteToBuffer((byte*)&value, 0, sizeof(long), endian);
        
        WriteBuffer(sizeof(long));
    }

    public void WriteUInt64(ulong value, Endian endian = Endian.Little)
    {
        WriteInt64((long)value, endian);
    }

    public void WriteSingle(float value, Endian endian = Endian.Little)
    {
        WriteToBuffer((byte*)&value, 0, sizeof(float), endian);
        
        WriteBuffer(sizeof(float));
    }
    
    public void WriteDouble(double value, Endian endian = Endian.Little)
    {
        WriteToBuffer((byte*)&value, 0, sizeof(double), endian);
        
        WriteBuffer(sizeof(double));
    }

    #endregion

    #region -- Strings --

    public void WriteNullTerminatedString(string value, bool wide = false)
    {
        byte[] tmp = new byte[(value.Length + 1) * (wide ? 2 : 1)];
        for (int i = 0; i < value.Length; i++)
        {
            tmp[i * (wide ? 2 : 1)] = (byte)value[i];
            if (wide)
            {
                tmp[i * 2 + 1] = (byte)((short)value[i] >> 8);
            }
        }
        m_stream.Write(tmp, 0, (value.Length + 1) * (wide ? 2 : 1));
    }

    public void WriteSizedString(string value)
    {
        Write7BitEncodedInt(value.Length);
        byte[] tmp = new byte[value.Length];
        for (int i = 0; i < value.Length; i++)
        {
            tmp[i] = (byte)value[i];
        }
        m_stream.Write(tmp, 0, value.Length);
    }

    #endregion

    public void WriteGuid(Guid value, Endian endian = Endian.Little)
    {
        WriteToBuffer((byte*)&value, 0, sizeof(int), endian);
        WriteToBuffer(((byte*)&value + 4), 4, sizeof(short), endian);
        WriteToBuffer(((byte*)&value + 6), 6, sizeof(short), endian);
        WriteToBuffer(((byte*)&value + 8), 8, 8, Endian.Little);
        WriteBuffer(sizeof(Guid));
    }
    
    public void Write7BitEncodedInt(int value)
    {
        uint v = (uint)value;
        while (v >= 0x80)
        {
            WriteByte((byte)(v | 0x80));
            v >>= 7;
        }
        WriteByte((byte)v);
    }

    public void Write7BitEncodedLong(long value)
    {
        ulong v = (ulong)value;
        while (v >= 0x80)
        {
            WriteByte((byte)(v | 0x80));
            v >>= 7;
        }
        WriteByte((byte)v);
    }

    public void WriteSha1(Sha1 value)
    {
        m_stream.Write(value.ToByteArray(), 0, 20);
    }

    #endregion

    public Stream CreateViewStream(long offset, long size)
    {
        m_stream.Position = offset;

        return new MemoryStream(ReadBytes((int)size));
    }
    
    public byte[] ToByteArray()
    {
        byte[] retVal = new byte[m_stream.Length];
        m_stream.Position = 0;
        int bytesRead = m_stream.Read(retVal, 0, (int)m_stream.Length);
        if (bytesRead != m_stream.Length)
        {
            throw new Exception();
        }
        return retVal;
    }
    
    public static implicit operator Stream(DataStream value) => value.m_stream;
    public static implicit operator DataStream(Stream value) => new(value);
    
    public void Dispose()
    {
        m_stream.Dispose();
        GC.SuppressFinalize(this);
    }
    
    private int FillBuffer(int count)
    {
        int read = m_stream.Read(m_buffer, 0, count);
        return read;
    }

    private void WriteToBuffer(byte* data, int offset, int count, Endian endian)
    {
        for (int i = 0; i < count; i++)
        {
            m_buffer[i + offset] = data[endian == Endian.Big ? count - i - 1 : i];
        }
    }
    
    private void WriteBuffer(int count)
    {
        m_stream.Write(m_buffer, 0, count);
    }
}
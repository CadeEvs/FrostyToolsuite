using System;

namespace Frosty.Sdk;

public readonly struct Sha1
{
    public static readonly Sha1 Zero = new();
    private readonly uint m_a, m_b, m_c, m_d, m_e;

    public Sha1(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length < 20)
        {
            throw new ArgumentException("Input buffer is too small");
        }

        m_a = (uint)(bytes[(0 * 4) + 0] | bytes[(0 * 4) + 1] << 8 | bytes[(0 * 4) + 2] << 16 | bytes[(0 * 4) + 3] << 24);
        m_b = (uint)(bytes[(1 * 4) + 0] | bytes[(1 * 4) + 1] << 8 | bytes[(1 * 4) + 2] << 16 | bytes[(1 * 4) + 3] << 24);
        m_c = (uint)(bytes[(2 * 4) + 0] | bytes[(2 * 4) + 1] << 8 | bytes[(2 * 4) + 2] << 16 | bytes[(2 * 4) + 3] << 24);
        m_d = (uint)(bytes[(3 * 4) + 0] | bytes[(3 * 4) + 1] << 8 | bytes[(3 * 4) + 2] << 16 | bytes[(3 * 4) + 3] << 24);
        m_e = (uint)(bytes[(4 * 4) + 0] | bytes[(4 * 4) + 1] << 8 | bytes[(4 * 4) + 2] << 16 | bytes[(4 * 4) + 3] << 24);
    }

    public Sha1(string text)
    {
        byte[] bytes = new byte[text.Length / 2];
        for (int i = 0; i < text.Length; i += 2)
            bytes[i / 2] = byte.Parse(text.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);

        if (bytes.Length < 20)
        {
            throw new ArgumentException("Input buffer is too small");
        }

        m_a = (uint)(bytes[(0 * 4) + 0] | bytes[(0 * 4) + 1] << 8 | bytes[(0 * 4) + 2] << 16 | bytes[(0 * 4) + 3] << 24);
        m_b = (uint)(bytes[(1 * 4) + 0] | bytes[(1 * 4) + 1] << 8 | bytes[(1 * 4) + 2] << 16 | bytes[(1 * 4) + 3] << 24);
        m_c = (uint)(bytes[(2 * 4) + 0] | bytes[(2 * 4) + 1] << 8 | bytes[(2 * 4) + 2] << 16 | bytes[(2 * 4) + 3] << 24);
        m_d = (uint)(bytes[(3 * 4) + 0] | bytes[(3 * 4) + 1] << 8 | bytes[(3 * 4) + 2] << 16 | bytes[(3 * 4) + 3] << 24);
        m_e = (uint)(bytes[(4 * 4) + 0] | bytes[(4 * 4) + 1] << 8 | bytes[(4 * 4) + 2] << 16 | bytes[(4 * 4) + 3] << 24);
    }

    public static bool operator ==(Sha1 a, Sha1 b) => a.Equals(b);
    public static bool operator !=(Sha1 a, Sha1 b) => !(a == b);

    public byte[] ToByteArray()
    {
        byte[] bytes = new byte[20];
        bytes[(0 * 4) + 0] = (byte)(m_a & 0xFF); bytes[(0 * 4) + 1] = (byte)((m_a >> 8) & 0xFF); bytes[(0 * 4) + 2] = (byte)((m_a >> 16) & 0xFF); bytes[(0 * 4) + 3] = (byte)((m_a >> 24) & 0xFF);
        bytes[(1 * 4) + 0] = (byte)(m_b & 0xFF); bytes[(1 * 4) + 1] = (byte)((m_b >> 8) & 0xFF); bytes[(1 * 4) + 2] = (byte)((m_b >> 16) & 0xFF); bytes[(1 * 4) + 3] = (byte)((m_b >> 24) & 0xFF);
        bytes[(2 * 4) + 0] = (byte)(m_c & 0xFF); bytes[(2 * 4) + 1] = (byte)((m_c >> 8) & 0xFF); bytes[(2 * 4) + 2] = (byte)((m_c >> 16) & 0xFF); bytes[(2 * 4) + 3] = (byte)((m_c >> 24) & 0xFF);
        bytes[(3 * 4) + 0] = (byte)(m_d & 0xFF); bytes[(3 * 4) + 1] = (byte)((m_d >> 8) & 0xFF); bytes[(3 * 4) + 2] = (byte)((m_d >> 16) & 0xFF); bytes[(3 * 4) + 3] = (byte)((m_d >> 24) & 0xFF);
        bytes[(4 * 4) + 0] = (byte)(m_e & 0xFF); bytes[(4 * 4) + 1] = (byte)((m_e >> 8) & 0xFF); bytes[(4 * 4) + 2] = (byte)((m_e >> 16) & 0xFF); bytes[(4 * 4) + 3] = (byte)((m_e >> 24) & 0xFF);
        return bytes;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != typeof(Sha1))
        {
            return false;
        }

        Sha1 otherSha1 = (Sha1)obj;
        if (m_a != otherSha1.m_a)
        {
            return false;
        }

        if (m_b != otherSha1.m_b)
        {
            return false;
        }

        if (m_c != otherSha1.m_c)
        {
            return false;
        }

        if (m_d != otherSha1.m_d)
        {
            return false;
        }

        if (m_e != otherSha1.m_e)
        {
            return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = (int)2166136261;
            hash = (hash * 16777619) ^ m_a.GetHashCode();
            hash = (hash * 16777619) ^ m_b.GetHashCode();
            hash = (hash * 16777619) ^ m_c.GetHashCode();
            hash = (hash * 16777619) ^ m_d.GetHashCode();
            hash = (hash * 16777619) ^ m_e.GetHashCode();
            return hash;
        }
    }

    public override string ToString()
    {
        string result = "";
        uint[] values = new uint[] { m_a, m_b, m_c, m_d, m_e };

        for (int i = 0; i < 5; i++)
            result += ((byte)(values[i] & 0xFF)).ToString("x2")
                + ((byte)(values[i] >> 8)).ToString("x2")
                + ((byte)(values[i] >> 16)).ToString("x2")
                + ((byte)(values[i] >> 24)).ToString("x2");

        return result;
    }
}
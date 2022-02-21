using System;

namespace FrostySdk
{
    public struct Sha1
    {
        public static readonly Sha1 Zero = new Sha1();
        private uint a, b, c, d, e;

        public Sha1(byte[] bytes)
        {
            if (bytes.Length < 20)
                throw new ArgumentException("Input buffer is too small");

            a = (uint)(bytes[(0 * 4) + 0] | bytes[(0 * 4) + 1] << 8 | bytes[(0 * 4) + 2] << 16 | bytes[(0 * 4) + 3] << 24);
            b = (uint)(bytes[(1 * 4) + 0] | bytes[(1 * 4) + 1] << 8 | bytes[(1 * 4) + 2] << 16 | bytes[(1 * 4) + 3] << 24);
            c = (uint)(bytes[(2 * 4) + 0] | bytes[(2 * 4) + 1] << 8 | bytes[(2 * 4) + 2] << 16 | bytes[(2 * 4) + 3] << 24);
            d = (uint)(bytes[(3 * 4) + 0] | bytes[(3 * 4) + 1] << 8 | bytes[(3 * 4) + 2] << 16 | bytes[(3 * 4) + 3] << 24);
            e = (uint)(bytes[(4 * 4) + 0] | bytes[(4 * 4) + 1] << 8 | bytes[(4 * 4) + 2] << 16 | bytes[(4 * 4) + 3] << 24);
        }

        public Sha1(string text)
        {
            byte[] bytes = new byte[text.Length / 2];
            for (int i = 0; i < text.Length; i += 2)
                bytes[i / 2] = byte.Parse(text.Substring(i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);

            if (bytes.Length < 20)
                throw new ArgumentException("Input buffer is too small");

            a = (uint)(bytes[(0 * 4) + 0] | bytes[(0 * 4) + 1] << 8 | bytes[(0 * 4) + 2] << 16 | bytes[(0 * 4) + 3] << 24);
            b = (uint)(bytes[(1 * 4) + 0] | bytes[(1 * 4) + 1] << 8 | bytes[(1 * 4) + 2] << 16 | bytes[(1 * 4) + 3] << 24);
            c = (uint)(bytes[(2 * 4) + 0] | bytes[(2 * 4) + 1] << 8 | bytes[(2 * 4) + 2] << 16 | bytes[(2 * 4) + 3] << 24);
            d = (uint)(bytes[(3 * 4) + 0] | bytes[(3 * 4) + 1] << 8 | bytes[(3 * 4) + 2] << 16 | bytes[(3 * 4) + 3] << 24);
            e = (uint)(bytes[(4 * 4) + 0] | bytes[(4 * 4) + 1] << 8 | bytes[(4 * 4) + 2] << 16 | bytes[(4 * 4) + 3] << 24);
        }

        public static bool operator ==(Sha1 A, Sha1 B) => ReferenceEquals(A, B) || (!ReferenceEquals(A, B) && A.Equals(B));
        public static bool operator !=(Sha1 A, Sha1 B) => !(A == B);

        public byte[] ToByteArray()
        {
            byte[] bytes = new byte[20];
            bytes[(0 * 4) + 0] = (byte)(a & 0xFF); bytes[(0 * 4) + 1] = (byte)((a >> 8) & 0xFF); bytes[(0 * 4) + 2] = (byte)((a >> 16) & 0xFF); bytes[(0 * 4) + 3] = (byte)((a >> 24) & 0xFF);
            bytes[(1 * 4) + 0] = (byte)(b & 0xFF); bytes[(1 * 4) + 1] = (byte)((b >> 8) & 0xFF); bytes[(1 * 4) + 2] = (byte)((b >> 16) & 0xFF); bytes[(1 * 4) + 3] = (byte)((b >> 24) & 0xFF);
            bytes[(2 * 4) + 0] = (byte)(c & 0xFF); bytes[(2 * 4) + 1] = (byte)((c >> 8) & 0xFF); bytes[(2 * 4) + 2] = (byte)((c >> 16) & 0xFF); bytes[(2 * 4) + 3] = (byte)((c >> 24) & 0xFF);
            bytes[(3 * 4) + 0] = (byte)(d & 0xFF); bytes[(3 * 4) + 1] = (byte)((d >> 8) & 0xFF); bytes[(3 * 4) + 2] = (byte)((d >> 16) & 0xFF); bytes[(3 * 4) + 3] = (byte)((d >> 24) & 0xFF);
            bytes[(4 * 4) + 0] = (byte)(e & 0xFF); bytes[(4 * 4) + 1] = (byte)((e >> 8) & 0xFF); bytes[(4 * 4) + 2] = (byte)((e >> 16) & 0xFF); bytes[(4 * 4) + 3] = (byte)((e >> 24) & 0xFF);
            return bytes;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Sha1))
                return false;

            Sha1 otherSha1 = (Sha1)obj;
            if (a != otherSha1.a) return false;
            if (b != otherSha1.b) return false;
            if (c != otherSha1.c) return false;
            if (d != otherSha1.d) return false;
            if (e != otherSha1.e) return false;

            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ a.GetHashCode();
                hash = (hash * 16777619) ^ b.GetHashCode();
                hash = (hash * 16777619) ^ c.GetHashCode();
                hash = (hash * 16777619) ^ d.GetHashCode();
                hash = (hash * 16777619) ^ e.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            string result = "";
            uint[] values = new uint[] { a, b, c, d, e };

            for (int i = 0; i < 5; i++)
                result += ((byte)(values[i] & 0xFF)).ToString("x2")
                    + ((byte)(values[i] >> 8)).ToString("x2")
                    + ((byte)(values[i] >> 16)).ToString("x2")
                    + ((byte)(values[i] >> 24)).ToString("x2");

            return result;
        }
    }
}

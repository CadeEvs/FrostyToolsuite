using System.Runtime.InteropServices;

namespace Frosty.Core
{
    /// <summary>
    /// Contains functions for working with half values.
    /// </summary>
    public class HalfUtils
    {

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatToUint
        {
            [FieldOffset(0)]
            public uint uintValue;
            [FieldOffset(0)]
            public float floatValue;
        }

        /// <summary>
        /// Unpacks the specified unsigned short value to its half equivalent.
        /// </summary>
        /// <param name="h">The unsigned short value.</param>
        /// <returns>A float representing the unpacked half.</returns>
        public static float Unpack(ushort h)
        {
            var conv = new FloatToUint {uintValue = HalfToFloatMantissaTable[HalfToFloatOffsetTable[h >> 10] + (((uint)h) & 0x3ff)] + HalfToFloatExponentTable[h >> 10]};
            return conv.floatValue;
        }

        /// <summary>
        /// Packs the specified half to its unsigned short equivalent.
        /// </summary>
        /// <param name="f">The half value.</param>
        /// <returns>An unsigned short representing the packed half.</returns>
        public static ushort Pack(float f)
        {
            FloatToUint conv = new FloatToUint {floatValue = f};
            return (ushort)(FloatToHalfBaseTable[(conv.uintValue >> 23) & 0x1ff] + ((conv.uintValue & 0x007fffff) >> FloatToHalfShiftTable[(conv.uintValue >> 23) & 0x1ff]));
        }

        static readonly uint[] HalfToFloatMantissaTable = new uint[2048];
        static readonly uint[] HalfToFloatExponentTable = new uint[64];
        static readonly uint[] HalfToFloatOffsetTable = new uint[64];
        static readonly ushort[] FloatToHalfBaseTable = new ushort[512];
        static readonly byte[] FloatToHalfShiftTable = new byte[512];

        static HalfUtils()
        {
            int i;

            // -------------------------------------------------------------------
            // Half to Float tables
            // -------------------------------------------------------------------

            // Mantissa table

            // 0 => 0
            HalfToFloatMantissaTable[0] = 0;

            // Transform subnormal to normalized
            for (i = 1; i < 1024; i++)
            {
                uint m = ((uint)i) << 13;
                uint e = 0;

                while ((m & 0x00800000) == 0)
                {
                    e -= 0x00800000;
                    m <<= 1;
                }
                m &= ~0x00800000U;
                e += 0x38800000;
                HalfToFloatMantissaTable[i] = m | e;
            }

            // Normal case
            for (i = 1024; i < 2048; i++)
                HalfToFloatMantissaTable[i] = 0x38000000 + (((uint)(i - 1024)) << 13);

            // Exponent table

            // 0 => 0
            HalfToFloatExponentTable[0] = 0;

            for (i = 1; i < 63; i++)
            {
                if (i < 31) // Positive Numbers
                    HalfToFloatExponentTable[i] = ((uint)i) << 23;
                else // Negative Numbers
                    HalfToFloatExponentTable[i] = 0x80000000 + (((uint)(i - 32)) << 23);
            }
            HalfToFloatExponentTable[31] = 0x47800000;
            HalfToFloatExponentTable[32] = 0x80000000;
            HalfToFloatExponentTable[63] = 0xC7800000;

            // Offset table
            HalfToFloatOffsetTable[0] = 0;
            for (i = 1; i < 64; i++)
                HalfToFloatOffsetTable[i] = 1024;
            HalfToFloatOffsetTable[32] = 0;

            // -------------------------------------------------------------------
            // Float to Half tables
            // -------------------------------------------------------------------

            for (i = 0; i < 256; i++)
            {
                int e = i - 127;
                if (e < -24)
                { // Very small numbers map to zero
                    FloatToHalfBaseTable[i | 0x000] = 0x0000;
                    FloatToHalfBaseTable[i | 0x100] = 0x8000;
                    FloatToHalfShiftTable[i | 0x000] = 24;
                    FloatToHalfShiftTable[i | 0x100] = 24;
                }
                else if (e < -14)
                { // Small numbers map to denorms
                    FloatToHalfBaseTable[i | 0x000] = (ushort)((0x0400 >> (-e - 14)));
                    FloatToHalfBaseTable[i | 0x100] = (ushort)((0x0400 >> (-e - 14)) | 0x8000);
                    FloatToHalfShiftTable[i | 0x000] = (byte)(-e - 1);
                    FloatToHalfShiftTable[i | 0x100] = (byte)(-e - 1);
                }
                else if (e <= 15)
                { // Normal numbers just lose precision
                    FloatToHalfBaseTable[i | 0x000] = (ushort)(((e + 15) << 10));
                    FloatToHalfBaseTable[i | 0x100] = (ushort)(((e + 15) << 10) | 0x8000);
                    FloatToHalfShiftTable[i | 0x000] = 13;
                    FloatToHalfShiftTable[i | 0x100] = 13;
                }
                else if (e < 128)
                { // Large numbers map to Infinity
                    FloatToHalfBaseTable[i | 0x000] = 0x7C00;
                    FloatToHalfBaseTable[i | 0x100] = 0xFC00;
                    FloatToHalfShiftTable[i | 0x000] = 24;
                    FloatToHalfShiftTable[i | 0x100] = 24;
                }
                else
                { // Infinity and NaN's stay Infinity and NaN's
                    FloatToHalfBaseTable[i | 0x000] = 0x7C00;
                    FloatToHalfBaseTable[i | 0x100] = 0xFC00;
                    FloatToHalfShiftTable[i | 0x000] = 13;
                    FloatToHalfShiftTable[i | 0x100] = 13;
                }
            }
        }
    }
}

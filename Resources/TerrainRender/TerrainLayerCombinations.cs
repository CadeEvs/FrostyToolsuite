using FrostySdk.IO;
using FrostySdk.Managers;
using FrostySdk.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using FrostySdk.Managers.Entries;

namespace LevelEditorPlugin.Resources
{
    internal unsafe class PtrUtils
    {
        public static string GetNullTerminatedString(byte* ptr)
        {
            StringBuilder sb = new StringBuilder();
            while (*ptr != 0x00)
            {
                sb.Append((char)*ptr);
                ptr++;
            }

            return sb.ToString();
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 4)]
    public unsafe struct TerrainLayerCombinationDrawPassData
    {
        public string ShaderName => PtrUtils.GetNullTerminatedString(StringPtr);

        [FieldOffset(0)]
        public byte* StringPtr;
        [FieldOffset(8)]
        public int RegularIndexCount;
        [FieldOffset(12)]
        public byte* RegularIndices;
        [FieldOffset(20)]
        public int MaskedIndexCount;
        [FieldOffset(24)]
        public byte* MaskedIndices;
        [FieldOffset(32)]
        public uint Unknown;
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct TerrainLayerCombinationDrawData
    {
        [FieldOffset(0)]
        public int IndexCount;
        [FieldOffset(4)]
        public byte* Indices;
        [FieldOffset(12)]
        public int MaskedDrawCount;
        [FieldOffset(16)]
        public TerrainLayerCombinationDrawPassData* MaskedDrawPass;
        [FieldOffset(24)]
        public int TwoDDrawCount;
        [FieldOffset(28)]
        public TerrainLayerCombinationDrawPassData* TwoDDrawPass;
        [FieldOffset(36)]
        public int DisplacementDrawCount;
        [FieldOffset(40)]
        public TerrainLayerCombinationDrawPassData* DisplacementDrawPass;
        [FieldOffset(48)]
        public int ThreeDDrawCount;
        [FieldOffset(52)]
        public TerrainLayerCombinationDrawPassData* ThreeDDrawPass;
        [FieldOffset(60)]
        public int Unknown2;
    }

    public unsafe class TerrainLayerCombinations : Resource
    {
        private unsafe struct MetaData
        {
            public int RelocationTableOffset => *(int*)&data[0];
            public int RelocationTableSize => *(int*)&data[4];

            private byte* data;

            internal MetaData(byte[] inData)
            {
                fixed (byte* ptr = &inData[0])
                {
                    data = ptr;
                }
            }
        }

        private byte* stringData => (byte*)&dataPtr[0];
        private TerrainLayerCombinationDrawData* drawDatas => *(TerrainLayerCombinationDrawData**)&dataPtr[0x0c];
        private int drawDataCount => *(int*)&dataPtr[0x08];

        private byte[] data;
        private byte* dataPtr;

        public override void Read(NativeReader reader, AssetManager am, ResAssetEntry entry, ModifiedResource modifiedData)
        {
            MetaData metaData = new MetaData(entry.ResMeta);

            data = reader.ReadBytes(metaData.RelocationTableOffset);
            fixed (byte* ptr = &data[0])
                dataPtr = ptr;

            Fixup(reader.ReadBytes(metaData.RelocationTableSize));

            //for (int i = 0; i < drawDataCount; i++)
            //{
            //    var drawData = drawDatas[i];
            //    System.Diagnostics.Debug.WriteLine($"{drawData.MaskedDrawCount},{drawData.TwoDDrawCount},{drawData.DisplacementDrawCount},{drawData.ThreeDDrawCount},{drawData.Unknown2}");
            //    System.Diagnostics.Debug.Write("  I: ");
            //    for (int j = 0; j < drawData.IndexCount; j++)
            //    {
            //        System.Diagnostics.Debug.Write($"{drawData.Indices[j]} ");
            //    }
            //    System.Diagnostics.Debug.Write("\n");

            //    for (int j = 0; j < drawData.MaskedDrawCount; j++)
            //    {
            //        System.Diagnostics.Debug.WriteLine($"  M: {drawData.MaskedDrawPass[j].Unknown.ToString("x8")} {drawData.MaskedDrawPass[j].ShaderName}");
            //    }
            //    for (int j = 0; j < drawData.TwoDDrawCount; j++)
            //    {
            //        System.Diagnostics.Debug.WriteLine($"  2: {drawData.TwoDDrawPass[j].Unknown.ToString("x8")} {drawData.TwoDDrawPass[j].ShaderName}");
            //    }
            //    for (int j = 0; j < drawData.DisplacementDrawCount; j++)
            //    {
            //        System.Diagnostics.Debug.WriteLine($"  D: {drawData.DisplacementDrawPass[j].Unknown.ToString("x8")} {drawData.DisplacementDrawPass[j].ShaderName}");
            //    }
            //    for (int j = 0; j < drawData.ThreeDDrawCount; j++)
            //    {
            //        System.Diagnostics.Debug.WriteLine($"  3: {drawData.ThreeDDrawPass[j].Unknown.ToString("x8")} {drawData.ThreeDDrawPass[j].ShaderName}");
            //    }
            //}
        }

        internal void Fixup(byte[] relocationTable)
        {
            fixed (byte** addrOfDataPtr = &dataPtr)
            {
                long startAddress = *(long*)addrOfDataPtr;
                for (int i = 0; i < relocationTable.Length / 4; i++)
                {
                    uint value = BitConverter.ToUInt32(relocationTable, i * 4);
                    byte* locationPtr = dataPtr + value;

                    *(long*)locationPtr += startAddress;
                }
            }
        }
    }
}

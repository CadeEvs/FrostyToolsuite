using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SoundEditorPlugin.WAV
{
    public class RIFFChunkHeader
    {
        public long StartOffset;
        public byte[] ChunkID;
        public uint PayloadSize;

        public string ChunkIDName
        {
            get
            {
                if (ChunkID == null) return "";
                return Encoding.ASCII.GetString(ChunkID);
            }
        }

        public RIFFChunkHeader(long startOffset, byte[] chunkID, uint payloadSize)
        {
            StartOffset = startOffset;
            ChunkID = chunkID;
            PayloadSize = payloadSize;
        }

        public RIFFChunkHeader(BinaryReader reader)
        {
            StartOffset = reader.BaseStream.Position;
            ChunkID = reader.ReadBytes(4);
            PayloadSize = reader.ReadUInt32();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(ChunkID);
            writer.Write(PayloadSize);
        }

        public override string ToString()
        {
            return ChunkIDName + " : " + PayloadSize + " bytes";
        }
    }

    // Apparently 'not really a chunk' but I call it one.
    // This class is used for reading RIFF files.
    public class RIFFMainChunk
    {
        public RIFFChunkHeader Header;

        public byte[] RIFFType;
        public List<RIFFChunkHeader> ChunkHeaders = new List<RIFFChunkHeader>();

        public string RIFFTypeName => Encoding.ASCII.GetString(RIFFType);

        public RIFFMainChunk(RIFFChunkHeader header, byte[] riffType)
        {
            Header = header;
            RIFFType = riffType;
        }

        public RIFFMainChunk(BinaryReader reader)
        {
            Header = new RIFFChunkHeader(reader);
            RIFFType = reader.ReadBytes(4);
            while (reader.BaseStream.Position < Header.StartOffset + 4 + 4 + Header.PayloadSize)
            {
                RIFFChunkHeader newHeader = new RIFFChunkHeader(reader);
                ChunkHeaders.Add(newHeader);
                reader.BaseStream.Seek(newHeader.PayloadSize, SeekOrigin.Current); // Skip over the payload to the next header.
            }
        }

        public void Write(BinaryWriter writer, List<IRIFFChunk> chunks)
        {
            uint size = 4;
            foreach (IRIFFChunk ch in chunks)
            {
                size += ch.Header.PayloadSize + 4 + 4;
            }
            Header.PayloadSize = size;
            Header.Write(writer);
            writer.Write(RIFFType);
            foreach (IRIFFChunk ch in chunks)
            {
                ch.Write(writer);
            }
        }

        public override string ToString()
        {
            return RIFFTypeName + " : " + ChunkHeaders.Count + " chunks : {" + string.Join(", ", ChunkHeaders) + "}";
        }
    }

    public interface IRIFFChunk
    {
        RIFFChunkHeader Header { get; set; }
        void Write(BinaryWriter writer);
    }

    public class WAVFormatChunk : IRIFFChunk
    {
        public RIFFChunkHeader Header { get; set; }

        // The full list of formats is mirrored at http://www-mmsp.ece.mcgill.ca/Documents/AudioFormats/WAVE/Docs/rfc2361.txt
        public enum DataFormats : ushort
        {
            WAVE_FORMAT_UNKNOWN = 0x0000,
            WAVE_FORMAT_PCM = 0x0001,
            WAVE_FORMAT_ADPCM = 0x0002,
            WAVE_FORMAT_IEEE_FLOAT = 0x0003,
            WAVE_FORMAT_VSELP = 0x0004,
            WAVE_FORMAT_IBM_CVSD = 0x0005,
            WAVE_FORMAT_ALAW = 0x0006,
            WAVE_FORMAT_MULAW = 0x0007,
            WAVE_FORMAT_OKI_ADPCM = 0x0010,
            WAVE_FORMAT_DVI_ADPCM = 0x0011,
            WAVE_FORMAT_MEDIASPACE_ADPCM = 0x0012,
            WAVE_FORMAT_SIERRA_ADPCM = 0x0013,
            WAVE_FORMAT_G723_ADPCM = 0x0014,
            WAVE_FORMAT_DIGISTD = 0x0015,
            WAVE_FORMAT_DIGIFIX = 0x0016,
            WAVE_FORMAT_DIALOGIC_OKI_ADPCM = 0x0017,
            WAVE_FORMAT_MEDIAVISION_ADPCM = 0x0018,
            WAVE_FORMAT_CU_CODEC = 0x0019,
            WAVE_FORMAT_YAMAHA_ADPCM = 0x0020,
            WAVE_FORMAT_SONARC = 0x0021,
            WAVE_FORMAT_DSPGROUP_TRUESPEECH = 0x0022,
            WAVE_FORMAT_ECHOSC1 = 0x0023,
            WAVE_FORMAT_AUDIOFILE_AF36 = 0x0024,
            WAVE_FORMAT_APTX = 0x0025,
            WAVE_FORMAT_AUDIOFILE_AF10 = 0x0026,
            WAVE_FORMAT_PROSODY_1612 = 0x0027,
            WAVE_FORMAT_LRC = 0x0028,
            WAVE_FORMAT_DOLBY_AC2 = 0x0030,
            WAVE_FORMAT_GSM610 = 0x0031,
            WAVE_FORMAT_MSNAUDIO = 0x0032,
            WAVE_FORMAT_ANTEX_ADPCME = 0x0033,
            WAVE_FORMAT_CONTROL_RES_VQLPC = 0x0034,
            WAVE_FORMAT_DIGIREAL = 0x0035,
            WAVE_FORMAT_DIGIADPCM = 0x0036,
            WAVE_FORMAT_CONTROL_RES_CR10 = 0x0037,
            WAVE_FORMAT_NMS_VBXADPCM = 0x0038,
            WAVE_FORMAT_ROLAND_RDAC = 0x0039,
            WAVE_FORMAT_ECHOSC3 = 0x003A,
            WAVE_FORMAT_ROCKWELL_ADPCM = 0x003B,
            WAVE_FORMAT_ROCKWELL_DIGITALK = 0x003C,
            WAVE_FORMAT_XEBEC = 0x003D,
            WAVE_FORMAT_G721_ADPCM = 0x0040,
            WAVE_FORMAT_G728_CELP = 0x0041,
            WAVE_FORMAT_MSG723 = 0x0042,
            WAVE_FORMAT_MPEG = 0x0050,
            WAVE_FORMAT_RT24 = 0x0052,
            WAVE_FORMAT_PAC = 0x0053,
            WAVE_FORMAT_MPEGLAYER3 = 0x0055,
            WAVE_FORMAT_LUCENT_G723 = 0x0059,
            WAVE_FORMAT_CIRRUS = 0x0060,
            WAVE_FORMAT_ESPCM = 0x0061,
            WAVE_FORMAT_VOXWARE = 0x0062,
            WAVE_FORMAT_CANOPUS_ATRAC = 0x0063,
            WAVE_FORMAT_G726_ADPCM = 0x0064,
            WAVE_FORMAT_G722_ADPCM = 0x0065,
            WAVE_FORMAT_DSAT = 0x0066,
            WAVE_FORMAT_DSAT_DISPLAY = 0x0067,
            WAVE_FORMAT_VOXWARE_BYTE_ALIGNED = 0x0069,
            WAVE_FORMAT_VOXWARE_AC8 = 0x0070,
            WAVE_FORMAT_VOXWARE_AC10 = 0x0071,
            WAVE_FORMAT_VOXWARE_AC16 = 0x0072,
            WAVE_FORMAT_VOXWARE_AC20 = 0x0073,
            WAVE_FORMAT_VOXWARE_RT24 = 0x0074,
            WAVE_FORMAT_VOXWARE_RT29 = 0x0075,
            WAVE_FORMAT_VOXWARE_RT29HW = 0x0076,
            WAVE_FORMAT_VOXWARE_VR12 = 0x0077,
            WAVE_FORMAT_VOXWARE_VR18 = 0x0078,
            WAVE_FORMAT_VOXWARE_TQ40 = 0x0079,
            WAVE_FORMAT_SOFTSOUND = 0x0080,
            WAVE_FORMAT_VOXWARE_TQ60 = 0x0081,
            WAVE_FORMAT_MSRT24 = 0x0082,
            WAVE_FORMAT_G729A = 0x0083,
            WAVE_FORMAT_MVI_MV12 = 0x0084,
            WAVE_FORMAT_DF_G726 = 0x0085,
            WAVE_FORMAT_DF_GSM610 = 0x0086,
            WAVE_FORMAT_ISIAUDIO = 0x0088,
            WAVE_FORMAT_ONLIVE = 0x0089,
            WAVE_FORMAT_SBC24 = 0x0091,
            WAVE_FORMAT_DOLBY_AC3_SPDIF = 0x0092,
            WAVE_FORMAT_ZYXEL_ADPCM = 0x0097,
            WAVE_FORMAT_PHILIPS_LPCBB = 0x0098,
            WAVE_FORMAT_PACKED = 0x0099,
            WAVE_FORMAT_RHETOREX_ADPCM = 0x0100,
            WAVE_FORMAT_IRAT = 0x0101,
            WAVE_FORMAT_VIVO_G723 = 0x00111,
            WAVE_FORMAT_VIVO_SIREN = 0x0112,
            WAVE_FORMAT_DIGITAL_G723 = 0x0123,
            WAVE_FORMAT_CREATIVE_ADPCM = 0x0200,
            WAVE_FORMAT_CREATIVE_FASTSPEECH8 = 0x0202,
            WAVE_FORMAT_CREATIVE_FASTSPEECH10 = 0x0203,
            WAVE_FORMAT_QUARTERDECK = 0x0220,
            WAVE_FORMAT_FM_TOWNS_SND = 0x0300,
            WAVE_FORMAT_BTV_DIGITAL = 0x0400,
            WAVE_FORMAT_VME_VMPCM = 0x0680,
            WAVE_FORMAT_OLIGSM = 0x1000,
            WAVE_FORMAT_OLIADPCM = 0x1001,
            WAVE_FORMAT_OLICELP = 0x1002,
            WAVE_FORMAT_OLISBC = 0x1003,
            WAVE_FORMAT_OLIOPR = 0x1004,
            WAVE_FORMAT_LH_CODEC = 0x1100,
            WAVE_FORMAT_NORRIS = 0x1400,
            WAVE_FORMAT_ISIAUDIO_2 = 0x1401, // Not sure why there are two identical names...
            WAVE_FORMAT_SOUNDSPACE_MUSICOMPRESS = 0x1500,
            WAVE_FORMAT_DVM = 0x2000,
            WAVE_FORMAT_EXTENSIBLE = 0xFFFE,
        }
        public DataFormats Format;
        public ushort ChannelCount;
        public uint SampleRate;
        public uint AverageBytesPerSecond;
        public ushort BlockAlign; // Bytes per frame, ChannelCount * BytesPerSample
        public ushort BitDepth; // Bits per sample.
        public ushort ExtensionSize; // bytes the follow this
        public ushort ValidBitsPerSample;
        public uint ChannelMask;
        public Guid SubFormat;

        public DataFormats SubFormatCode
        {
            get => (DataFormats)(BitConverter.ToUInt16(SubFormat.ToByteArray(), 0));
            set
            {
                // Change the first two bytes of SubFormat.
                byte[] data = SubFormat.ToByteArray();
                byte[] newdata = BitConverter.GetBytes((ushort)value);
                data[0] = newdata[0];
                data[1] = newdata[1];
                SubFormat = new Guid(data);
            }
        }

        public WAVFormatChunk(DataFormats format, ushort channelCount, uint sampleRate, uint averageBytesPerSecond, ushort blockAlign, ushort bitDepth)
        {
            Header = new RIFFChunkHeader(0, new byte[] { 0x66, 0x6D, 0x74, 0x20 }, 16);
            Format = format;
            ChannelCount = channelCount;
            SampleRate = sampleRate;
            AverageBytesPerSecond = averageBytesPerSecond;
            BlockAlign = blockAlign;
            BitDepth = bitDepth;
        }

        public WAVFormatChunk(RIFFChunkHeader header, BinaryReader reader)
        {
            Header = header;
            reader.BaseStream.Position = header.StartOffset + 4 + 4; // past chunk name and size
            Format = (DataFormats)reader.ReadUInt16();
            ChannelCount = reader.ReadUInt16();
            SampleRate = reader.ReadUInt32();
            AverageBytesPerSecond = reader.ReadUInt32();
            BlockAlign = reader.ReadUInt16();
            BitDepth = reader.ReadUInt16();
            if (Header.PayloadSize > 16)
            {
                // Read extended format information
                ExtensionSize = reader.ReadUInt16();
                if (ExtensionSize > 0)
                {
                    ValidBitsPerSample = reader.ReadUInt16();
                    ChannelMask = reader.ReadUInt32();
                    SubFormat = new Guid(reader.ReadBytes(16));
                }
            }
        }

        public void Write(BinaryWriter writer)
        {
            Header.Write(writer);
            writer.Write((ushort)Format);
            writer.Write(ChannelCount);
            writer.Write(SampleRate);
            writer.Write(AverageBytesPerSecond);
            writer.Write(BlockAlign);
            writer.Write(BitDepth);
            if (Header.PayloadSize > 16)
            {
                writer.Write(ExtensionSize);
                if (ExtensionSize > 0)
                {
                    writer.Write(ValidBitsPerSample);
                    writer.Write(ChannelMask);
                    writer.Write(SubFormat.ToByteArray());
                }
            }
        }
    }

    public class WAVDataChunk : IRIFFChunk
    {
        public RIFFChunkHeader Header { get; set; }
        public WAVFormatChunk Format;

        public List<WAVDataFrame> Frames;

        public WAVDataChunk(WAVFormatChunk format, List<WAVDataFrame> frames)
        {
            Format = format;
            Frames = frames;
            Header = new RIFFChunkHeader(0, new byte[] { 0x64, 0x61, 0x74, 0x61 }, (uint)(Frames.Count * Format.BitDepth / 8 * Format.ChannelCount));
        }

        public WAVDataChunk(RIFFChunkHeader header, WAVFormatChunk format, BinaryReader reader)
        {
            Header = header;
            Format = format;
            uint frameCount = (uint)((Header.PayloadSize / Format.ChannelCount) / (Format.BitDepth / 8));

            reader.BaseStream.Position = Header.StartOffset + 4 + 4;

            Frames = new List<WAVDataFrame>();
            for (uint frame = 0; frame < frameCount; frame++)
            {
                WAVDataFrame f;
                if (Format.Format == WAVFormatChunk.DataFormats.WAVE_FORMAT_PCM)
                    if (Format.BitDepth == 8)
                        f = new WAV8BitDataFrame(Format.ChannelCount);
                    else if (Format.BitDepth == 16)
                        f = new WAV16BitDataFrame(Format.ChannelCount);
                    else if (Format.BitDepth == 32)
                        f = new WAV32BitDataFrame(Format.ChannelCount);
                    else
                        throw new Exception("Invalid bit depth: " + Format.BitDepth);
                else
                    throw new Exception("Cannot read non-PCM data. This data is format " + Format.Format.ToString());
                f.Read(reader);
                Frames.Add(f);
            }
        }

        public void Write(BinaryWriter writer)
        {
            Header.Write(writer);
            foreach (WAVDataFrame frame in Frames)
            {
                frame.Write(writer);
            }
        }
    }

    public abstract class WAVDataFrame
    {
        public abstract ushort BitDepth { get; }
        public ushort ChannelCount { get; private set; }

        public WAVDataFrame(ushort channelCount)
        {
            ChannelCount = channelCount;
        }

        public abstract void Read(BinaryReader reader);
        public abstract void Write(BinaryWriter writer);
        public abstract object GetSample(ushort channel);
    }

    public class WAV8BitDataFrame : WAVDataFrame
    {
        public override ushort BitDepth => 8;

        public byte[] Data;

        public WAV8BitDataFrame(ushort channelCount) : base(channelCount)
        {
            Data = new byte[channelCount];
        }

        public override object GetSample(ushort channel)
        {
            return Data[channel];
        }

        public override void Read(BinaryReader reader)
        {
            for (int i = 0; i < ChannelCount; i++)
            {
                Data[i] = reader.ReadByte();
            }
        }

        public override void Write(BinaryWriter writer)
        {
            for (int i = 0; i < ChannelCount; i++)
            {
                writer.Write(Data[i]);
            }
        }
    }

    public class WAV16BitDataFrame : WAVDataFrame
    {
        public override ushort BitDepth => 16;

        public short[] Data;

        public WAV16BitDataFrame(ushort channelCount) : base(channelCount)
        {
            Data = new short[channelCount];
        }

        public override object GetSample(ushort channel)
        {
            return Data[channel];
        }

        public override void Read(BinaryReader reader)
        {
            for (int i = 0; i < ChannelCount; i++)
            {
                Data[i] = reader.ReadInt16();
            }
        }

        public override void Write(BinaryWriter writer)
        {
            for (int i = 0; i < ChannelCount; i++)
            {
                writer.Write(Data[i]);
            }
        }
    }

    public class WAV32BitDataFrame : WAVDataFrame
    {
        public override ushort BitDepth => 32;

        public float[] Data;

        public WAV32BitDataFrame(ushort channelCount) : base(channelCount)
        {
            Data = new float[channelCount];
        }

        public override object GetSample(ushort channel)
        {
            return Data[channel];
        }

        public override void Read(BinaryReader reader)
        {
            for (int i = 0; i < ChannelCount; i++)
            {
                Data[i] = reader.ReadSingle();
            }
        }

        public override void Write(BinaryWriter writer)
        {
            for (int i = 0; i < ChannelCount; i++)
            {
                writer.Write(Data[i]);
            }
        }
    }
}

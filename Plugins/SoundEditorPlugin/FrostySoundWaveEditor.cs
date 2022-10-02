using System;
using System.Collections.Generic;
using FrostySdk.Interfaces;
using System.Windows;
using FrostySdk.IO;
using System.IO;
using FrostySdk;
using FrostySdk.Managers;
using FrostySdk.Ebx;
using WaveFormRendererLib;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using NAudio.Wave;
using Frosty.Core.Controls;
using Frosty.Core;
using Frosty.Core.Windows;
using FrostySdk.Managers.Entries;
using SoundEditorPlugin.Resources;

namespace SoundEditorPlugin
{
    public class FrostySoundWaveEditor : FrostySoundDataEditor
    {
        public FrostySoundWaveEditor()
            : base(null)
        {
        }

        public FrostySoundWaveEditor(ILogger inLogger)
            : base(inLogger)
        {
        }

        protected override List<SoundDataTrack> InitialLoad(FrostyTaskWindow task)
        {
            List<SoundDataTrack> retVal = new List<SoundDataTrack>();
            dynamic soundWave = RootObject;

            int index = 0;
            int totalCount = soundWave.RuntimeVariations.Count;

            foreach (dynamic runtimeVariation in soundWave.RuntimeVariations)
            {
                task.Update(status: "Loading track #" + (index + 1), progress: ((index + 1) / (double)totalCount) * 100.0d);

                SoundDataTrack track = new SoundDataTrack { Name = "Track #" + ((index++) + 1) };

                dynamic soundDataChunk = soundWave.Chunks[runtimeVariation.ChunkIndex];
                ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(soundDataChunk.ChunkId);
                if (chunkEntry == null)
                {
                    continue;
                }

                using (NativeReader reader = new NativeReader(App.AssetManager.GetChunk(chunkEntry)))
                {
                    List<short> decodedSoundBuf = new List<short>();
                    double startLoopingTime = 0.0;
                    double loopingDuration = 0.0;

                    for (int i = 0; i < runtimeVariation.SegmentCount; i++)
                    {
                        var segment = soundWave.Segments[runtimeVariation.FirstSegmentIndex + i];
                        reader.Position = segment.SamplesOffset;

                        if (reader.ReadUShort() != 0x48)
                        {
                            logger.LogError("Wrong Sample Offset at Variation {0}, Segment {1}", index, i);
                            return retVal;
                        }

                        ushort headersize = reader.ReadUShort(Endian.Big);
                        byte codec = (byte)(reader.ReadByte() & 0xF);
                        int channels = (reader.ReadByte() >> 2) + 1;
                        ushort sampleRate = reader.ReadUShort(Endian.Big);
                        uint sampleCount = reader.ReadUInt(Endian.Big) & 0xFFFFFFF;
                        //reader.Position += headersize - 0x0C;
                        switch (codec)
                        {
                            case 0x1: track.Codec = "Unknown"; break;
                            case 0x2: track.Codec = "PCM 16 Big"; break;
                            case 0x3: track.Codec = "EA-XMA"; break;
                            case 0x4: track.Codec = "XAS Interleaved v1"; break;
                            case 0x5: track.Codec = "EALayer3 Interleaved v1"; break;
                            case 0x6: track.Codec = "EALayer3 Interleaved v2 PCM"; break;
                            case 0x7: track.Codec = "EALayer3 Interleaved v2 Spike"; break;
                            case 0x9: track.Codec = "EASpeex"; break;
                            case 0xA: track.Codec = "Unknown"; break;
                            case 0xB: track.Codec = "EA-MP3"; break;
                            case 0xC: track.Codec = "EAOpus"; break;
                            case 0xD: track.Codec = "EAAtrac9"; break;
                            case 0xE: track.Codec = "MultiStream Opus"; break;
                            case 0xF: track.Codec = "MultiStream Opus (Uncoupled)"; break;
                        }

                        if (i == runtimeVariation.FirstLoopSegmentIndex && runtimeVariation.SegmentCount > 1)
                        {
                            startLoopingTime = (decodedSoundBuf.Count / channels) / (double)sampleRate;
                            track.LoopStart = (uint)decodedSoundBuf.Count;
                        }

                        reader.Position = segment.SamplesOffset;
                        byte[] soundBuf = reader.ReadToEnd();

                        if (codec == 0x2)
                        {
                            short[] data = Pcm16b.Decode(soundBuf);
                            decodedSoundBuf.AddRange(data);
                            sampleCount = (uint)data.Length;
                        }
                        else if (codec == 0x4)
                        {
                            short[] data = XAS.Decode(soundBuf);
                            decodedSoundBuf.AddRange(data);
                            sampleCount = (uint)data.Length;
                        }
                        else if (codec == 0x5 || codec == 0x6)
                        {
                            sampleCount = 0;
                            EALayer3.Decode(soundBuf, soundBuf.Length, (short[] data, int count, EALayer3.StreamInfo info) =>
                            {
                                if (info.streamIndex == -1)
                                    return;
                                sampleCount += (uint)data.Length;
                                decodedSoundBuf.AddRange(data);
                            });
                        }

                        if (i == runtimeVariation.LastLoopSegmentIndex && runtimeVariation.SegmentCount > 1)
                        {
                            loopingDuration = ((decodedSoundBuf.Count / channels) / (double)sampleRate) - startLoopingTime;
                            track.LoopEnd = (uint)decodedSoundBuf.Count;
                        }

                        track.SampleRate = sampleRate;
                        track.ChannelCount = channels;
                        if (segment.SegmentLength == 0)
                            segment.SegmentLength = (decodedSoundBuf.Count / track.ChannelCount) / (float)sampleRate;
                    }
                    track.Duration = (decodedSoundBuf.Count / track.ChannelCount) / (double)track.SampleRate;
                    //track.LoopEnd += track.LoopStart;
                    track.Samples = decodedSoundBuf.ToArray();

                    var maxPeakProvider = new MaxPeakProvider();
                    var rmsPeakProvider = new RmsPeakProvider(200); // e.g. 200
                    var samplingPeakProvider = new SamplingPeakProvider(200); // e.g. 200
                    var averagePeakProvider = new AveragePeakProvider(4); // e.g. 4

                    var topSpacerColor = System.Drawing.Color.FromArgb(64, 83, 22, 3);
                    var soundCloudOrangeTransparentBlocks = new SoundCloudBlockWaveFormSettings(System.Drawing.Color.FromArgb(255, 218, 218, 218), topSpacerColor, System.Drawing.Color.FromArgb(255, 109, 109, 109),
                                                                                                System.Drawing.Color.FromArgb(64, 79, 79, 79))
                    {
                        Name = "SoundCloud Orange Transparent Blocks",
                        PixelsPerPeak = 2,
                        SpacerPixels = 1,
                        TopSpacerGradientStartColor = topSpacerColor,
                        BackgroundColor = System.Drawing.Color.FromArgb(128, 0, 0, 0),
                        Width = 800,
                        TopHeight = 49,
                        BottomHeight = 29,
                    };

                    try
                    {
                        var renderer = new WaveFormRenderer();
                        var image = renderer.Render(track.Samples, maxPeakProvider, soundCloudOrangeTransparentBlocks);

                        using (var ms = new MemoryStream())
                        {
                            image.Save(ms, ImageFormat.Png);
                            ms.Seek(0, SeekOrigin.Begin);

                            var bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.StreamSource = ms;
                            bitmapImage.EndInit();

                            var target = new RenderTargetBitmap(bitmapImage.PixelWidth, bitmapImage.PixelHeight, bitmapImage.DpiX, bitmapImage.DpiY, PixelFormats.Pbgra32);
                            var visual = new DrawingVisual();

                            using (var r = visual.RenderOpen())
                            {
                                visual.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                                r.DrawImage(bitmapImage, new Rect(0, 0, bitmapImage.Width, bitmapImage.Height));

                                if (loopingDuration > 0)
                                {
                                    r.DrawLine(new Pen(Brushes.White, 1.0),
                                        new Point((int)((startLoopingTime / track.Duration) * soundCloudOrangeTransparentBlocks.Width), soundCloudOrangeTransparentBlocks.TopHeight),
                                        new Point((int)((startLoopingTime / track.Duration) * soundCloudOrangeTransparentBlocks.Width), (int)bitmapImage.Height));
                                    r.DrawLine(new Pen(Brushes.White, 1.0),
                                        new Point((int)(((startLoopingTime + loopingDuration) / track.Duration) * soundCloudOrangeTransparentBlocks.Width), soundCloudOrangeTransparentBlocks.TopHeight),
                                        new Point((int)(((startLoopingTime + loopingDuration) / track.Duration) * soundCloudOrangeTransparentBlocks.Width), (int)bitmapImage.Height));
                                    r.DrawLine(new Pen(Brushes.White, 1.0),
                                        new Point((int)((startLoopingTime / track.Duration) * soundCloudOrangeTransparentBlocks.Width), (int)bitmapImage.Height),
                                        new Point((int)(((startLoopingTime + loopingDuration) / track.Duration) * soundCloudOrangeTransparentBlocks.Width), (int)bitmapImage.Height));
                                }
                            }

                            target.Render(visual);
                            target.Freeze();
                            track.WaveForm = target;
                        }
                    }
                    catch (Exception e)
                    {
                    }

                    track.SegmentCount = runtimeVariation.SegmentCount;
                }

                retVal.Add(track);
            }

            foreach (dynamic localization in soundWave.Localization)
            {
                for (int i = 0; i < localization.VariationCount; i++)
                {
                    SoundDataTrack track = retVal[i + localization.FirstVariationIndex];

                    PointerRef pr = localization.Language;
                    EbxAsset asset = App.AssetManager.GetEbx(App.AssetManager.GetEbxEntry(pr.External.FileGuid));
                    dynamic obj = asset.GetObject(pr.External.ClassGuid);

                    track.Language = obj.__Id;
                }
            }

            return retVal;
        }
    }

    public class FrostyNewWaveEditor : FrostySoundDataEditor
    {
        public FrostyNewWaveEditor()
            : base(null)
        {
        }

        public FrostyNewWaveEditor(ILogger inLogger)
            : base(inLogger)
        {
        }

        protected override List<SoundDataTrack> InitialLoad(FrostyTaskWindow task)
        {
            List<SoundDataTrack> retVal = new List<SoundDataTrack>();
            dynamic root = RootObject;
            NewWaveResource newWave = App.AssetManager.GetResAs<NewWaveResource>(App.AssetManager.GetResEntry(((string)(root.Name)).ToLower()));

            int index = 0;
            int totalCount = newWave.Variations.Count;

            foreach (dynamic runtimeVariation in newWave.Variations)
            {
                task.Update(status: "Loading track #" + (index + 1), progress: ((index + 1) / (double)totalCount) * 100.0d);
                SoundDataTrack track = new SoundDataTrack { Name = "Track #" + ((index++) + 1) };

                int chunkIndex = newWave.Segments[(int)runtimeVariation.FirstSegmentIndex].SamplesOffsetFlag == 1 ? (int)runtimeVariation.MemoryChunkIndex : (int)runtimeVariation.StreamChunkIndex;
                dynamic soundDataChunk = newWave.Chunks[chunkIndex];
                ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(soundDataChunk.ChunkId);
                if (chunkEntry == null)
                {
                    continue;
                }
                using (NativeReader reader = new NativeReader(App.AssetManager.GetChunk(chunkEntry)))
                {
                    List<short> decodedSoundBuf = new List<short>();
                    double startLoopingTime = 0.0;
                    double loopingDuration = 0.0;

                    for (int i = 0; i < runtimeVariation.SegmentCount; i++)
                    {
                        var segment = newWave.Segments[(int)runtimeVariation.FirstSegmentIndex + i];
                        reader.Position = segment.SamplesOffset;

                        if (reader.ReadUShort() != 0x48)
                        {
                            logger.LogError("Wrong Sample Offset at Variation {0}, Segment {1}", index, i);
                            return retVal;
                        }

                        ushort headersize = reader.ReadUShort(Endian.Big);
                        byte codec = (byte)(reader.ReadByte() & 0xF);
                        int channels = (reader.ReadByte() >> 2) + 1;
                        ushort sampleRate = reader.ReadUShort(Endian.Big);
                        uint sampleCount = reader.ReadUInt(Endian.Big) & 0xFFFFFFF;
                        //reader.Position += headersize - 0x0C;
                        switch (codec)
                        {
                            case 0x1: track.Codec = "Unknown"; break;
                            case 0x2: track.Codec = "PCM 16 Big"; break;
                            case 0x3: track.Codec = "EA-XMA"; break;
                            case 0x4: track.Codec = "XAS Interleaved v1"; break;
                            case 0x5: track.Codec = "EALayer3 Interleaved v1"; break;
                            case 0x6: track.Codec = "EALayer3 Interleaved v2 PCM"; break;
                            case 0x7: track.Codec = "EALayer3 Interleaved v2 Spike"; break;
                            case 0x9: track.Codec = "EASpeex"; break;
                            case 0xA: track.Codec = "Unknown"; break;
                            case 0xB: track.Codec = "EA-MP3"; break;
                            case 0xC: track.Codec = "EAOpus"; break;
                            case 0xD: track.Codec = "EAAtrac9"; break;
                            case 0xE: track.Codec = "MultiStream Opus"; break;
                            case 0xF: track.Codec = "MultiStream Opus (Uncoupled)"; break;
                        }

                        if (i == runtimeVariation.FirstLoopSegmentIndex && runtimeVariation.SegmentCount > 1)
                        {
                            startLoopingTime = (decodedSoundBuf.Count / channels) / (double)sampleRate;
                            track.LoopStart = (uint)decodedSoundBuf.Count;
                        }

                        reader.Position = segment.SamplesOffset;
                        byte[] soundBuf = reader.ReadToEnd();
                        double duration = 0.0;

                        if (codec == 0x2)
                        {
                            short[] data = Pcm16b.Decode(soundBuf);
                            decodedSoundBuf.AddRange(data);
                            duration += (data.Length / channels) / (double)sampleRate;
                            sampleCount = (uint)data.Length;
                        }
                        else if (codec == 0x4)
                        {
                            short[] data = XAS.Decode(soundBuf);
                            decodedSoundBuf.AddRange(data);
                            duration += (data.Length / channels) / (double)sampleRate;
                            sampleCount = (uint)data.Length;
                        }
                        else if (codec == 0x5 || codec == 0x6 || codec == 0xC)
                        {
                            sampleCount = 0;
                            EALayer3.Decode(soundBuf, soundBuf.Length, (short[] data, int count, EALayer3.StreamInfo info) =>
                            {
                                if (info.streamIndex == -1)
                                    return;
                                sampleCount += (uint)data.Length;
                                decodedSoundBuf.AddRange(data);
                            });
                            duration += (sampleCount / channels) / (double)sampleRate;
                        }

                        if (runtimeVariation.SegmentCount > 1)
                        {
                            if (i < runtimeVariation.FirstLoopSegmentIndex)
                            {
                                startLoopingTime += duration;
                                track.LoopStart += sampleCount;
                            }
                            if (i >= runtimeVariation.FirstLoopSegmentIndex && i <= runtimeVariation.LastLoopSegmentIndex)
                            {
                                loopingDuration += duration;
                                track.LoopEnd += sampleCount;
                            }
                        }

                        track.SampleRate = sampleRate;
                        track.ChannelCount = channels;
                        track.Duration += duration;
                    }

                    track.LoopEnd += track.LoopStart;
                    track.Samples = decodedSoundBuf.ToArray();

                    var maxPeakProvider = new MaxPeakProvider();
                    var rmsPeakProvider = new RmsPeakProvider(200); // e.g. 200
                    var samplingPeakProvider = new SamplingPeakProvider(200); // e.g. 200
                    var averagePeakProvider = new AveragePeakProvider(4); // e.g. 4

                    var topSpacerColor = System.Drawing.Color.FromArgb(64, 83, 22, 3);
                    var soundCloudOrangeTransparentBlocks = new SoundCloudBlockWaveFormSettings(System.Drawing.Color.FromArgb(255, 218, 218, 218), topSpacerColor, System.Drawing.Color.FromArgb(255, 109, 109, 109),
                                                                                                System.Drawing.Color.FromArgb(64, 79, 79, 79))
                    {
                        Name = "SoundCloud Orange Transparent Blocks",
                        PixelsPerPeak = 2,
                        SpacerPixels = 1,
                        TopSpacerGradientStartColor = topSpacerColor,
                        BackgroundColor = System.Drawing.Color.FromArgb(128, 0, 0, 0),
                        Width = 800,
                        TopHeight = 49,
                        BottomHeight = 29,
                    };

                    try
                    {
                        var renderer = new WaveFormRenderer();
                        var image = renderer.Render(track.Samples, maxPeakProvider, soundCloudOrangeTransparentBlocks);

                        using (var ms = new MemoryStream())
                        {
                            image.Save(ms, ImageFormat.Png);
                            ms.Seek(0, SeekOrigin.Begin);

                            var bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.StreamSource = ms;
                            bitmapImage.EndInit();

                            var target = new RenderTargetBitmap(bitmapImage.PixelWidth, bitmapImage.PixelHeight, bitmapImage.DpiX, bitmapImage.DpiY, PixelFormats.Pbgra32);
                            var visual = new DrawingVisual();

                            using (var r = visual.RenderOpen())
                            {
                                visual.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                                r.DrawImage(bitmapImage, new Rect(0, 0, bitmapImage.Width, bitmapImage.Height));

                                if (loopingDuration > 0)
                                {
                                    r.DrawLine(new Pen(Brushes.White, 1.0),
                                        new Point((int)((startLoopingTime / track.Duration) * soundCloudOrangeTransparentBlocks.Width), soundCloudOrangeTransparentBlocks.TopHeight),
                                        new Point((int)((startLoopingTime / track.Duration) * soundCloudOrangeTransparentBlocks.Width), (int)bitmapImage.Height));
                                    r.DrawLine(new Pen(Brushes.White, 1.0),
                                        new Point((int)(((startLoopingTime + loopingDuration) / track.Duration) * soundCloudOrangeTransparentBlocks.Width), soundCloudOrangeTransparentBlocks.TopHeight),
                                        new Point((int)(((startLoopingTime + loopingDuration) / track.Duration) * soundCloudOrangeTransparentBlocks.Width), (int)bitmapImage.Height));
                                    r.DrawLine(new Pen(Brushes.White, 1.0),
                                        new Point((int)((startLoopingTime / track.Duration) * soundCloudOrangeTransparentBlocks.Width), (int)bitmapImage.Height),
                                        new Point((int)(((startLoopingTime + loopingDuration) / track.Duration) * soundCloudOrangeTransparentBlocks.Width), (int)bitmapImage.Height));
                                }
                            }

                            target.Render(visual);
                            target.Freeze();
                            track.WaveForm = target;
                        }
                    }
                    catch (Exception e)
                    {
                    }

                    track.SegmentCount = (int)runtimeVariation.SegmentCount;
                }

                retVal.Add(track);
            }

            return retVal;
        }
    }

    public class FrostyHarmonySampleBankEditor : FrostySoundDataEditor
    {
        public FrostyHarmonySampleBankEditor()
            : base(null)
        {
        }

        public FrostyHarmonySampleBankEditor(ILogger inLogger)
            : base(inLogger)
        {
        }

        protected override List<SoundDataTrack> InitialLoad(FrostyTaskWindow task)
        {
            List<SoundDataTrack> retVal = new List<SoundDataTrack>();

            dynamic soundWave = RootObject;
            dynamic ramChunk = soundWave.Chunks[soundWave.RamChunkIndex];


            int index = 0;

            ChunkAssetEntry ramChunkEntry = App.AssetManager.GetChunkEntry(ramChunk.ChunkId);


            NativeReader streamChunkReader = null;
            if (soundWave.StreamChunkIndex != 255)
            {
                dynamic streamChunk = soundWave.Chunks[soundWave.StreamChunkIndex];
                ChunkAssetEntry streamChunkEntry = App.AssetManager.GetChunkEntry(streamChunk.ChunkId);
                streamChunkReader = new NativeReader(App.AssetManager.GetChunk(streamChunkEntry));
            }

            using (NativeReader reader = new NativeReader(App.AssetManager.GetChunk(ramChunkEntry)))
            {
                reader.Position = 0x0a;
                int datasetCount = reader.ReadUShort();

                reader.Position = 0x20;
                int dataOffset = reader.ReadInt();

                reader.Position = 0x50;
                List<int> offsets = new List<int>();
                for (int i = 0; i < datasetCount; i++)
                {
                    offsets.Add(reader.ReadInt());
                    reader.Position += 4;
                }

                foreach (int offset in offsets)
                {
                    reader.Position = offset + 0x3c;
                    int blockCount = reader.ReadUShort();
                    reader.Position += 0x0a;

                    int fileOffset = -1;
                    bool streaming = false;

                    for (int i = 0; i < blockCount; i++)
                    {
                        uint blockType = reader.ReadUInt();
                        if (blockType == 0x2e4f4646)
                        {
                            reader.Position += 4;
                            fileOffset = reader.ReadInt();
                            reader.Position += 0x0c;

                            streaming = true;
                        }
                        else if (blockType == 0x2e52414d)
                        {
                            reader.Position += 4;
                            fileOffset = reader.ReadInt() + dataOffset;
                            reader.Position += 0x0c;
                        }
                        else
                        {
                            reader.Position += 0x14;
                        }
                    }

                    if (fileOffset != -1)
                    {
                        NativeReader actualReader = reader;
                        if (streaming)
                            actualReader = streamChunkReader;

                        SoundDataTrack track = new SoundDataTrack { Name = "Track #" + (index++) };

                        actualReader.Position = fileOffset;
                        List<short> decodedSoundBuf = new List<short>();

                        uint headerSize = actualReader.ReadUInt(Endian.Big) & 0x00ffffff;
                        byte codec = actualReader.ReadByte();
                        int channels = (actualReader.ReadByte() >> 2) + 1;
                        ushort sampleRate = actualReader.ReadUShort(Endian.Big);
                        uint sampleCount = actualReader.ReadUInt(Endian.Big) & 0x00ffffff;

                        switch (codec)
                        {
                            case 0x14: track.Codec = "XAS"; break;
                            case 0x15: track.Codec = "EALayer3 v5"; break;
                            case 0x16: track.Codec = "EALayer3 v6"; break;
                            default: track.Codec = "Unknown (" + codec.ToString("x2") + ")"; break;
                        }

                        actualReader.Position = fileOffset;
                        byte[] soundBuf = actualReader.ReadToEnd();
                        double duration = 0.0;

                        if (codec == 0x14)
                        {
                            short[] data = XAS.Decode(soundBuf);
                            decodedSoundBuf.AddRange(data);
                            duration += (data.Length / channels) / (double)sampleRate;
                        }
                        else if (codec == 0x15 || codec == 0x16)
                        {
                            sampleCount = 0;
                            EALayer3.Decode(soundBuf, soundBuf.Length, (short[] data, int count, EALayer3.StreamInfo info) =>
                            {
                                if (info.streamIndex == -1)
                                    return;
                                sampleCount += (uint)data.Length;
                                decodedSoundBuf.AddRange(data);
                            });
                            duration += (sampleCount / channels) / (double)sampleRate;
                        }

                        track.Duration += duration;
                        track.Samples = decodedSoundBuf.ToArray();

                        var maxPeakProvider = new MaxPeakProvider();
                        var rmsPeakProvider = new RmsPeakProvider(200); // e.g. 200
                        var samplingPeakProvider = new SamplingPeakProvider(200); // e.g. 200
                        var averagePeakProvider = new AveragePeakProvider(4); // e.g. 4

                        var topSpacerColor = System.Drawing.Color.FromArgb(64, 83, 22, 3);
                        var soundCloudOrangeTransparentBlocks = new SoundCloudBlockWaveFormSettings(System.Drawing.Color.FromArgb(196, 197, 53, 0), topSpacerColor, System.Drawing.Color.FromArgb(196, 79, 26, 0),
                                                                                                    System.Drawing.Color.FromArgb(64, 79, 79, 79))
                        {
                            Name = "SoundCloud Orange Transparent Blocks",
                            PixelsPerPeak = 2,
                            SpacerPixels = 1,
                            TopSpacerGradientStartColor = topSpacerColor,
                            BackgroundColor = System.Drawing.Color.Transparent,
                            Width = 800,
                            TopHeight = 50,
                            BottomHeight = 30,
                        };

                        try
                        {
                            var renderer = new WaveFormRenderer();
                            var image = renderer.Render(decodedSoundBuf.ToArray(), maxPeakProvider, soundCloudOrangeTransparentBlocks);

                            using (var ms = new MemoryStream())
                            {
                                image.Save(ms, ImageFormat.Png);
                                ms.Seek(0, SeekOrigin.Begin);

                                var bitmapImage = new BitmapImage();
                                bitmapImage.BeginInit();
                                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                bitmapImage.StreamSource = ms;
                                bitmapImage.EndInit();

                                var target = new RenderTargetBitmap(bitmapImage.PixelWidth, bitmapImage.PixelHeight, bitmapImage.DpiX, bitmapImage.DpiY, PixelFormats.Pbgra32);
                                var visual = new DrawingVisual();

                                using (var r = visual.RenderOpen())
                                {
                                    r.DrawImage(bitmapImage, new Rect(0, 0, bitmapImage.Width, bitmapImage.Height));
                                }

                                target.Render(visual);
                                target.Freeze();
                                track.WaveForm = target;
                            }
                        }
                        catch (Exception e)
                        {
                        }

                        track.SegmentCount = 1;
                        retVal.Add(track);
                    }
                }
            }

            return retVal;
        }
    }

    public class FrostyOctaneSoundEditor : FrostySoundDataEditor
    {
        public FrostyOctaneSoundEditor()
            : base(null)
        {
        }

        public FrostyOctaneSoundEditor(ILogger inLogger)
            : base(inLogger)
        {
        }

        private static int[] EA_XA_TABLE = 
        {
            0,  240,  460,  392,
            0,    0, -208, -220,
            0,    1,    3,    4,
            7,    8,   10,   11,
            0,   -1,   -3,   -4
        };

        protected override List<SoundDataTrack> InitialLoad(FrostyTaskWindow task)
        {
            dynamic octane = RootObject;
            List<SoundDataTrack> retVal = new List<SoundDataTrack>();

            dynamic soundDataChunkId = octane.Chunks[0].ChunkId;

            SoundDataTrack track = new SoundDataTrack() { ChannelCount = 1, Codec = "XAS Interleaved v0", Name = "Track #1" };
            double startLoopingTime = 0;
            double loopingDuration = 0;
            List<short> decodedSoundBuf = new List<short>();

            MemoryStream file = App.AssetManager.GetChunk(App.AssetManager.GetChunkEntry(soundDataChunkId));
            using (NativeReader reader = new NativeReader(file))
            {
                string magic = reader.ReadSizedString(4);
                uint version = reader.ReadUInt();
                float minrpm = reader.ReadFloat();
                float maxrpm = reader.ReadFloat();
                uint table1Size = reader.ReadUInt();
                uint table2Size = reader.ReadUInt();
                uint sampleCount = reader.ReadUInt();
                track.SampleRate = reader.ReadInt();

                //rpm to sample table
                reader.ReadInt();
                int[] table1 = new int[table1Size];
                for (int i = 0; i < table1Size; i++)
                {
                    table1[i] = reader.ReadInt();
                }

                //sample to sampleloop table
                reader.ReadInt();
                int[] table2 = new int[table2Size];
                for (int i = 0; i < table2Size; i++)
                {
                    table2[i] = reader.ReadInt();
                }

                long pos = reader.Position;
                if (pos != ((table1Size + 1 + table2Size + 1) * 4) + 32)
                {
                    logger.LogError("Wrong offset after Tables");
                    pos = ((table1Size + 1 + table2Size + 1) * 4) + 32;
                }

                for (int block = 0; block < (reader.Length - pos) / 19; block++)
                {
                    short[] blocksamples = new short[32];
                    uint header = reader.ReadUInt(Endian.Little);
                    int coef1 = EA_XA_TABLE[(header & 0x0F) + 0];
                    int coef2 = EA_XA_TABLE[(header & 0x0F) + 4];
                    short hist1 = (short)((header >> 16) & 0xFFF0);
                    short hist2 = (short)((header >> 0) & 0xFFF0);
                    byte shift = (byte)((header >> 16) & 0x0F);
                    blocksamples[0] = hist2;
                    blocksamples[1] = hist1;

                    for (int row = 0; row < 15; row++)
                    {
                        byte b = reader.ReadByte();
                        for (int i = 0; i < 2; i++)
                        {
                            int sample = 0;
                            if (i == 0)
                                sample = (b & 0xF0) >> 4;
                            else if (i == 1)
                                sample = b & 0x0F;

                            if (sample > 7)
                                sample -= 16;

                            int num = hist1 * coef1 + hist2 * coef2;

                            sample = ((sample << (20 - shift)) + num + 128) >> 8;

                            // Clamp sample
                            if (sample > short.MaxValue)
                                sample = short.MaxValue;
                            else if (sample < short.MinValue)
                                sample = short.MinValue;

                            blocksamples[2 + row * 2 + i] = (short)sample;

                            hist2 = hist1;
                            hist1 = (short)sample;
                        }
                    }
                    decodedSoundBuf.AddRange(blocksamples);
                }
                track.Samples = decodedSoundBuf.ToArray();
                track.Duration = decodedSoundBuf.Count / track.SampleRate;
                retVal.Add(track);
            }
            var maxPeakProvider = new MaxPeakProvider();
            var rmsPeakProvider = new RmsPeakProvider(200); // e.g. 200
            var samplingPeakProvider = new SamplingPeakProvider(200); // e.g. 200
            var averagePeakProvider = new AveragePeakProvider(4); // e.g. 4

            var topSpacerColor = System.Drawing.Color.FromArgb(64, 83, 22, 3);
            var soundCloudOrangeTransparentBlocks = new SoundCloudBlockWaveFormSettings(System.Drawing.Color.FromArgb(255, 218, 218, 218), topSpacerColor, System.Drawing.Color.FromArgb(255, 109, 109, 109),
                                                                                        System.Drawing.Color.FromArgb(64, 79, 79, 79))
            {
                Name = "SoundCloud Orange Transparent Blocks",
                PixelsPerPeak = 2,
                SpacerPixels = 1,
                TopSpacerGradientStartColor = topSpacerColor,
                BackgroundColor = System.Drawing.Color.FromArgb(128, 0, 0, 0),
                Width = 800,
                TopHeight = 49,
                BottomHeight = 29,
            };

            try
            {
                var renderer = new WaveFormRenderer();
                var image = renderer.Render(track.Samples, maxPeakProvider, soundCloudOrangeTransparentBlocks);

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = ms;
                    bitmapImage.EndInit();

                    var target = new RenderTargetBitmap(bitmapImage.PixelWidth, bitmapImage.PixelHeight, bitmapImage.DpiX, bitmapImage.DpiY, PixelFormats.Pbgra32);
                    var visual = new DrawingVisual();

                    using (var r = visual.RenderOpen())
                    {
                        visual.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                        r.DrawImage(bitmapImage, new Rect(0, 0, bitmapImage.Width, bitmapImage.Height));

                        if (loopingDuration > 0)
                        {
                            r.DrawLine(new Pen(Brushes.White, 1.0),
                                new Point((int)((startLoopingTime / track.Duration) * soundCloudOrangeTransparentBlocks.Width), soundCloudOrangeTransparentBlocks.TopHeight),
                                new Point((int)((startLoopingTime / track.Duration) * soundCloudOrangeTransparentBlocks.Width), (int)bitmapImage.Height));
                            r.DrawLine(new Pen(Brushes.White, 1.0),
                                new Point((int)(((startLoopingTime + loopingDuration) / track.Duration) * soundCloudOrangeTransparentBlocks.Width), soundCloudOrangeTransparentBlocks.TopHeight),
                                new Point((int)(((startLoopingTime + loopingDuration) / track.Duration) * soundCloudOrangeTransparentBlocks.Width), (int)bitmapImage.Height));
                            r.DrawLine(new Pen(Brushes.White, 1.0),
                                new Point((int)((startLoopingTime / track.Duration) * soundCloudOrangeTransparentBlocks.Width), (int)bitmapImage.Height),
                                new Point((int)(((startLoopingTime + loopingDuration) / track.Duration) * soundCloudOrangeTransparentBlocks.Width), (int)bitmapImage.Height));
                        }
                    }

                    target.Render(visual);
                    target.Freeze();
                    track.WaveForm = target;
                }
            }
            catch (Exception e)
            {
            }

            return retVal;
        }
    }

    public class FrostyImpulseResponseEditor : FrostySoundDataEditor
    {
        public FrostyImpulseResponseEditor()
            : base(null)
        {
        }

        public FrostyImpulseResponseEditor(ILogger inLogger)
            : base(inLogger)
        {
        }
    }
}

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

        //public override List<ToolbarItem> RegisterToolbarItems()
        //{
        //    return new List<ToolbarItem>()
        //    {
        //        // dev import, only handles first variations
        //        new ToolbarItem("Import", "Import", "", new RelayCommand((o) =>
        //        {
        //            FrostyOpenFileDialog ofd = new FrostyOpenFileDialog("Import Sound", "*.mp3|*.mp3", "Sound");
        //            if (!ofd.ShowDialog())
        //                return;

        //            using (var reader = new MediaFoundationReader(ofd.FileName))
        //            {
        //                int totalSamples = 0;
        //                using (var writer = new NativeWriter(new MemoryStream()))
        //                {
        //                    writer.Write(0x4800000c, Endian.Big);
        //                    writer.Write((byte)0x12);
        //                    writer.Write((byte)((reader.WaveFormat.Channels - 1) << 2));
        //                    writer.Write((ushort)(reader.WaveFormat.SampleRate), Endian.Big);

        //                    long pos=writer.Position;
        //                    writer.Write(0x40000000, Endian.Big);

        //                    while (reader.Position < reader.Length)
        //                    {
        //                        int bufLength = 0x2600 * 2 * reader.WaveFormat.Channels;
        //                        if (totalSamples + 0x2600 > 0x00ffffff)
        //                            break;

        //                        byte[] buf = new byte[bufLength];

        //                        int actualRead = reader.Read(buf, 0, bufLength);
        //                        if (actualRead == 0)
        //                            break;

        //                        writer.Write((actualRead + 8) | 0x44000000, Endian.Big);
        //                        writer.Write(((actualRead / reader.WaveFormat.Channels) / 2), Endian.Big);

        //                        for (int i = 0; i < actualRead/2; i++)
        //                        {
        //                            short s = BitConverter.ToInt16(buf, i * 2);
        //                            writer.Write(s, Endian.Big);
        //                        }

        //                        totalSamples += ((actualRead / reader.WaveFormat.Channels) / 2);
        //                    }

        //                    writer.Write(0x45000004, Endian.Big);
        //                    writer.Position=pos;
        //                    writer.Write(totalSamples | 0x40000000, Endian.Big);

        //                    byte[] resultBuf = writer.ToByteArray();;

        //                    dynamic soundWave = RootObject;
        //                    dynamic soundDataChunk = soundWave.Chunks[0];
        //                    ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(soundDataChunk.ChunkId);

        //                    bool modify = true;
        //                    if (modify)
        //                    {
        //                        App.AssetManager.ModifyChunk(chunkEntry.Id, writer.ToByteArray());
        //                        soundDataChunk.ChunkSize = (uint)resultBuf.Length;
        //                    }
        //                    else
        //                    {
        //                        Guid chunkId = App.AssetManager.AddChunk(resultBuf);

        //                        ChunkAssetEntry newEntry = App.AssetManager.GetChunkEntry(chunkId);
        //                        newEntry.AddToBundles(chunkEntry.Bundles);

        //                        soundDataChunk = TypeLibrary.CreateObject("SoundDataChunk");
        //                        soundDataChunk.ChunkId = chunkId;

        //                        soundWave.Chunks.Add(soundDataChunk);

        //                        dynamic segment = TypeLibrary.CreateObject("SoundWaveVariationSegment");
        //                        segment.SeekTableOffset = 4294967295;
        //                        soundWave.Segments.Add(segment);

        //                        dynamic variation = TypeLibrary.CreateObject("SoundWaveRuntimeVariation");
        //                        variation.FirstSegmentIndex = (ushort)(soundWave.Segments.Count - 1);
        //                        variation.SegmentCount = (byte)1;
        //                        variation.ChunkIndex = (byte)(soundWave.Chunks.Count - 1);
        //                        variation.Weight = (byte)100;
        //                        soundWave.RuntimeVariations.Add(variation);
        //                    }

        //                    InvokeOnAssetModified();
        //                }
        //            }
        //        }))
        //    };
        //}

        protected override List<SoundDataTrack> InitialLoad(FrostyTaskWindow task)
        {
            List<SoundDataTrack> retVal = new List<SoundDataTrack>();
            dynamic soundWave = RootObject;

            int index = 0;
            int totalCount = soundWave.RuntimeVariations.Count;

            foreach (dynamic runtimeVariation in soundWave.RuntimeVariations)
            {
                task.Update(status: "Loading track #" + (index + 1), progress: ((index + 1) / (double)totalCount) * 100.0d);

                SoundDataTrack track = new SoundDataTrack {Name = "Track #" + ((index++) + 1)};

                dynamic soundDataChunk = soundWave.Chunks[runtimeVariation.ChunkIndex];
                ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(soundDataChunk.ChunkId);

                if (chunkEntry == null)
                {
                    App.Logger.LogWarning($"SoundChunk {soundDataChunk.ChunkId} doesn't exist. This could be because its a LocalizedChunk that is not loaded by your game.");
                }
                else
                {
                    using (NativeReader reader = new NativeReader(App.AssetManager.GetChunk(chunkEntry)))
                    {
                        List<short> decodedSoundBuf = new List<short>();
                        double startLoopingTime = 0.0;
                        double loopingDuration = 0.0;

                        for (int i = 0; i < runtimeVariation.SegmentCount; i++)
                        {
                            var segment = soundWave.Segments[runtimeVariation.FirstSegmentIndex + i];
                            reader.Position = segment.SamplesOffset;

                            uint headerSize = reader.ReadUInt(Endian.Big) & 0x00ffffff;
                            byte codec = reader.ReadByte();
                            int channels = (reader.ReadByte() >> 2) + 1;
                            ushort sampleRate = reader.ReadUShort(Endian.Big);
                            uint sampleCount = reader.ReadUInt(Endian.Big) & 0x00ffffff;

                            switch (codec)
                            {
                                case 0x12: track.Codec = "Pcm16Big"; break;
                                case 0x14: track.Codec = "Xas1"; break;
                                case 0x15: track.Codec = "EaLayer31"; break;
                                case 0x16: track.Codec = "EaLayer32Pcm"; break;
                                default: track.Codec = "Unknown (" + codec.ToString("x2") + ")"; break;
                            }

                            reader.Position = segment.SamplesOffset;
                            long size = reader.Length - reader.Position;
                            if ((runtimeVariation.FirstSegmentIndex + i + 1 < soundWave.Segments.Count) && ((soundWave.Segments[runtimeVariation.FirstSegmentIndex + i + 1].SamplesOffset > segment.SamplesOffset)))
                            {
                                size = soundWave.Segments[runtimeVariation.FirstSegmentIndex + i + 1].SamplesOffset - reader.Position;
                            }
                            byte[] soundBuf = reader.ReadBytes((int)size);
                            double duration = 0.0;

                            if (codec == 0x12)
                            {
                                short[] data = Pcm16b.Decode(soundBuf);
                                decodedSoundBuf.AddRange(data);
                                duration += (data.Length / channels) / (double)sampleRate;
                                sampleCount = (uint)data.Length;
                            }
                            else if (codec == 0x14)
                            {
                                short[] data = XAS.Decode(soundBuf);
                                decodedSoundBuf.AddRange(data);
                                duration += (data.Length / channels) / (double)sampleRate;
                                sampleCount = (uint)data.Length;
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
            dynamic newWave = RootObject;

            int index = 0;
            foreach (dynamic soundDataChunk in newWave.Chunks)
            {
                SoundDataTrack track = new SoundDataTrack {Name = "Track #" + ((index++) + 1)};

                ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(soundDataChunk.ChunkId);

                if (chunkEntry == null)
                {
                    App.Logger.LogWarning($"SoundChunk {soundDataChunk.ChunkId} doesn't exist. This could be because its a LocalizedChunk that is not loaded by your game.");
                }
                else
                {
                    using (NativeReader reader = new NativeReader(App.AssetManager.GetChunk(chunkEntry)))
                    {
                        List<short> decodedSoundBuf = new List<short>();
                        reader.Position = 0;

                        uint headerSize = reader.ReadUInt(Endian.Big) & 0x00ffffff;
                        byte codec = reader.ReadByte();
                        int channels = (reader.ReadByte() >> 2) + 1;
                        ushort sampleRate = reader.ReadUShort(Endian.Big);
                        uint sampleCount = reader.ReadUInt(Endian.Big) & 0x00ffffff;

                        switch (codec)
                        {
                            case 0x12: track.Codec = "Pcm16Big"; break;
                            case 0x14: track.Codec = "Xas1"; break;
                            case 0x15: track.Codec = "EaLayer31"; break;
                            case 0x16: track.Codec = "EaLayer32Pcm"; break;
                            case 0x1c: track.Codec = "EaOpus"; break;
                            default: track.Codec = "Unknown (" + codec.ToString("x2") + ")"; break;
                        }

                        reader.Position = 0;
                        byte[] soundBuf = reader.ReadToEnd();
                        double duration = 0.0;

                        if (codec == 0x12)
                        {
                            short[] data = Pcm16b.Decode(soundBuf);
                            decodedSoundBuf.AddRange(data);
                            duration += (data.Length / channels) / (double)sampleRate;
                        }
                        else if (codec == 0x14)
                        {
                            short[] data = XAS.Decode(soundBuf);
                            decodedSoundBuf.AddRange(data);
                            duration += (data.Length / channels) / (double)sampleRate;
                        }
                        else if (codec == 0x15 || codec == 0x16 || codec == 0x1c)
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

                        track.SampleRate = sampleRate;
                        track.ChannelCount = channels;
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
                    }
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

                        SoundDataTrack track = new SoundDataTrack {Name = "Track #" + (index++)};

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

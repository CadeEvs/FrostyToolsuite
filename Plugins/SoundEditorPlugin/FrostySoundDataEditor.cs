using Frosty.Core.Controls;
using Frosty.Core.Windows;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using SharpDX;
using SharpDX.Multimedia;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Frosty.Core;
using FrostySdk.Managers;
using FrostySdk.Managers.Entries;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using WaveFormatExtensible = SharpDX.Multimedia.WaveFormatExtensible;

namespace SoundEditorPlugin
{
    public static class EALayer3
    {
        public delegate void AudioCallback([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] short[] data, int count, StreamInfo info);

        [StructLayout(LayoutKind.Sequential)]
        public struct StreamInfo
        {
            public int streamIndex;
            public int numChannels;
            public int sampleRate;
        }

        [DllImport("../thirdparty/ealayer3.dll", EntryPoint = "Decode")]
        public static extern void Decode([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] buffer, int length, AudioCallback callback);
    }

    public static class Pcm16b
    {
        public static short[] Decode(byte[] soundBuffer)
        {
            using (NativeReader reader = new NativeReader(new MemoryStream(soundBuffer)))
            {
                ushort blockType = reader.ReadUShort();
                ushort blockSize = reader.ReadUShort(Endian.Big);
                byte compressionType = reader.ReadByte();

                int channelCount = (reader.ReadByte() >> 2) + 1;
                ushort sampleRate = reader.ReadUShort(Endian.Big);
                int totalSampleCount = reader.ReadInt(Endian.Big) & 0x00ffffff;

                List<short>[] channels = new List<short>[channelCount];
                for (int i = 0; i < channelCount; i++)
                    channels[i] = new List<short>();

                while (reader.Position <= reader.Length)
                {
                    blockType = reader.ReadUShort();
                    blockSize = reader.ReadUShort(Endian.Big);

                    if (blockType == 0x45)
                        break;

                    uint samples = reader.ReadUInt(Endian.Big);

                    for (int i = 0; i < samples; i++)
                    {
                        for (int j = 0; j < channelCount; j++)
                            channels[j].Add(reader.ReadShort(Endian.Big));
                    }
                }

                short[] outBuffer = new short[channels[0].Count * channelCount];
                for (int i = 0; i < channels[0].Count; i++)
                {
                    for (int j = 0; j < channelCount; j++)
                    {
                        outBuffer[(i * channelCount) + j] = channels[j][i];
                    }
                }

                return outBuffer;
            }
        }
    }
    public static class XAS
    {
        public static short[] Decode(byte[] soundBuffer)
        {
            using (NativeReader reader = new NativeReader(new MemoryStream(soundBuffer)))
            {
                ushort blockType = reader.ReadUShort();
                ushort blockSize = reader.ReadUShort(Endian.Big);
                byte compressionType = reader.ReadByte();

                int channelCount = (reader.ReadByte() >> 2) + 1;
                ushort sampleRate = reader.ReadUShort(Endian.Big);
                int totalSampleCount = reader.ReadInt(Endian.Big) & 0x00ffffff;

                List<short>[] channels = new List<short>[channelCount];
                for (int i = 0; i < channelCount; i++)
                    channels[i] = new List<short>();

                while (reader.Position <= reader.Length)
                {
                    blockType = reader.ReadUShort();
                    blockSize = reader.ReadUShort(Endian.Big);

                    if (blockType == 0x45)
                        break;

                    uint samples = reader.ReadUInt(Endian.Big);

                    byte[] buffer = null;
                    short[] blockBuffer = new short[32];
                    int[] consts1 = new int[4] { 0, 240, 460, 392 };
                    int[] consts2 = new int[4] { 0, 0, -208, -220 };

                    for (int i = 0; i < (blockSize / 76 / channelCount); i++)
                    {
                        for (int j = 0; j < channelCount; j++)
                        {
                            buffer = reader.ReadBytes(76);

                            for (int k = 0; k < 4; k++)
                            {
                                blockBuffer[0] = (short)(buffer[k * 4 + 0] & 0xF0 | buffer[k * 4 + 1] << 8);
                                blockBuffer[1] = (short)(buffer[k * 4 + 2] & 0xF0 | buffer[k * 4 + 3] << 8);

                                int index4 = (int)buffer[k * 4] & 0x0F;
                                int num10 = (int)buffer[k * 4 + 2] & 0x0F;
                                int index5 = 2;

                                while (index5 < 32)
                                {
                                    int num11 = ((int)buffer[12 + k + index5 * 2] & 240) >> 4;
                                    if (num11 > 7)
                                        num11 -= 16;

                                    int num12 = blockBuffer[index5 - 1] * consts1[index4] + blockBuffer[index5 - 2] * consts2[index4];

                                    blockBuffer[index5] = (short)(num12 + (num11 << 20 - num10) + 128 >> 8);
                                    if (blockBuffer[index5] > (int)short.MaxValue)
                                        blockBuffer[index5] = (int)short.MaxValue;
                                    else if (blockBuffer[index5] < (int)short.MinValue)
                                        blockBuffer[index5] = (int)short.MinValue;

                                    int num13 = (int)buffer[12 + k + index5 * 2] & 15;
                                    if (num13 > 7)
                                        num13 -= 16;

                                    int num14 = blockBuffer[index5] * consts1[index4] + blockBuffer[index5 - 1] * consts2[index4];

                                    blockBuffer[index5 + 1] = (short)(num14 + (num13 << 20 - num10) + 128 >> 8);
                                    if (blockBuffer[index5 + 1] > (int)short.MaxValue)
                                        blockBuffer[index5 + 1] = (int)short.MaxValue;
                                    else if (blockBuffer[index5 + 1] < (int)short.MinValue)
                                        blockBuffer[index5 + 1] = (int)short.MinValue;

                                    index5 += 2;
                                }

                                channels[j].AddRange(blockBuffer);
                            }

                            uint sampleSize = (samples < 128) ? samples : 128;
                            samples -= sampleSize;
                        }
                    }
                }

                short[] outBuffer = new short[channels[0].Count * channelCount];
                for (int i = 0; i < channels[0].Count; i++)
                {
                    for (int j = 0; j < channelCount; j++)
                    {
                        outBuffer[(i * channelCount) + j] = channels[j][i];
                    }
                }

                return outBuffer;
            }
        }
    }

    public class SoundDataTrack : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Codec { get; set; }
        public double Duration { get; set; }
        public int SegmentCount { get; set; }
        public string Language { get; set; }
        public ImageSource WaveForm { get; set; }
        public int SampleRate { get; set; }
        public int ChannelCount { get; set; }
        public short[] Samples { get; set; }
        public uint LoopStart { get; set; }
        public uint LoopEnd { get; set; }

        public double Progress { get => progress; set { progress = value; NotifyPropertyChanged(); } }
        private double progress;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SoundWave : IDisposable
    {
        public SoundDataTrack track;
        public event RoutedEventHandler OnFinishedPlaying;
        public double Progress => (voice.State.SamplesPlayed - (loopPtr / track.ChannelCount) < 0) ? voice.State.SamplesPlayed / (double)(SampleCount / track.ChannelCount) : (voice.State.SamplesPlayed - (loopPtr / track.ChannelCount)) / (double)(SampleCount / track.ChannelCount);
        public long SampleCount => track.Samples.Length;

        private SourceVoice voice;
        private AudioBuffer buffer;
        private int bufferPtr;
        private long loopPtr;
        private long loopCount;

        public SoundWave(SoundDataTrack inTrack, AudioPlayer player)
        {
            track = inTrack;

            WaveFormatExtensible format = new WaveFormatExtensible(track.SampleRate, 16, track.ChannelCount);
            switch (track.ChannelCount)
            {
                case 2: format.ChannelMask = Speakers.FrontLeft | Speakers.FrontRight; break;
                case 4: format.ChannelMask = Speakers.FrontLeft | Speakers.FrontRight | Speakers.BackLeft | Speakers.BackRight; break;
                case 6: format.ChannelMask = Speakers.FrontLeft | Speakers.FrontRight | Speakers.FrontCenter | Speakers.LowFrequency | Speakers.BackLeft | Speakers.BackRight; break;
                default: format.ChannelMask = 0; break;
            }

            voice = new SourceVoice(player.AudioSystem, format, true);
            voice.SetOutputVoices(new VoiceSendDescriptor(player.OutputVoice));
            voice.BufferEnd += Voice_BufferEnd;

            Voice_BufferEnd(IntPtr.Zero);

            voice.Start();
        }

        private const int MAX_BUFFER_SIZE = 4096;
        private void Voice_BufferEnd(IntPtr obj)
        {
            if (bufferPtr < SampleCount)
            {
                int bufferSize = (SampleCount - bufferPtr > MAX_BUFFER_SIZE * track.ChannelCount) ? MAX_BUFFER_SIZE * track.ChannelCount : (int)(SampleCount - bufferPtr);
                DataStream DS = new DataStream(bufferSize * sizeof(short), true, true);
                buffer = new AudioBuffer
                {
                    Stream = DS,
                    AudioBytes = (int)DS.Length,
                    Flags = BufferFlags.None
                };

                // interleave channels
                while (DS.Position < DS.Length)
                {
                    DS.Write(track.Samples[bufferPtr]);
                    bufferPtr++;

                    if (track.LoopEnd != 0 && bufferPtr == track.LoopEnd)
                    {
                        loopPtr += bufferPtr - track.LoopStart;
                        loopCount++;

                        bufferPtr = (int)track.LoopStart;
                    }
                }

                voice.SubmitSourceBuffer(buffer, null);
            }
            else
            {
                OnFinishedPlaying?.Invoke(this, new RoutedEventArgs());
            }
        }

        public void Dispose()
        {
            voice.Stop();
            voice.DestroyVoice();
            voice.Dispose();
        }
    }

    public class AudioPlayer : IDisposable
    {
        public XAudio2 AudioSystem { get; }
        public MasteringVoice OutputVoice { get; }
        public double Progress => currentSound?.Progress ?? 0.0;
        public bool IsPlaying { get; private set; }

        private SoundWave currentSound;

        public AudioPlayer()
        {
            AudioSystem = new XAudio2();
            OutputVoice = new MasteringVoice(AudioSystem, 8);
        }

        public void PlaySound(SoundDataTrack track)
        {
            SoundDispose();

            currentSound = new SoundWave(track, this);
            IsPlaying = true;
            currentSound.OnFinishedPlaying += CurrentSound_OnFinishedPlaying;
        }

        private void CurrentSound_OnFinishedPlaying(object sender, RoutedEventArgs e)
        {
            IsPlaying = false;
            //SoundDispose();
        }

        public void SoundDispose()
        {
            IsPlaying = false;

            if (currentSound == null)
                return;

            var tmpSound = currentSound;
            currentSound = null;
            tmpSound.Dispose();
        }

        public void Dispose()
        {
            SoundDispose();

            OutputVoice.Dispose();
            AudioSystem.Dispose();
        }
    }

    [TemplatePart(Name = PART_TracksListBox, Type = typeof(ListView))]
    [TemplatePart(Name = PART_PlayButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_StopButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_VolumeSlider, Type = typeof(Slider))]
    [TemplatePart(Name = PART_SoundExportMenuItem, Type = typeof(MenuItem))]
    [TemplatePart(Name = PART_SoundImportMenuItem, Type = typeof(MenuItem))]
    public class FrostySoundDataEditor : FrostyAssetEditor
    {
        private const string PART_TracksListBox = "PART_TracksListBox";
        private const string PART_PlayButton = "PART_PlayButton";
        private const string PART_StopButton = "PART_StopButton";
        private const string PART_VolumeSlider = "PART_VolumeSlider";
        private const string PART_SoundExportMenuItem = "PART_SoundExportMenuItem";
        private const string PART_SoundImportMenuItem = "PART_SoundImportMenuItem";

        public static readonly DependencyProperty TracksListProperty = DependencyProperty.Register("TracksList", typeof(ObservableCollection<SoundDataTrack>), typeof(FrostySoundDataEditor), new FrameworkPropertyMetadata(null));
        public ObservableCollection<SoundDataTrack> TracksList
        {
            get => (ObservableCollection<SoundDataTrack>)GetValue(TracksListProperty);
            set => SetValue(TracksListProperty, value);
        }

        public bool IsPlaying => audioPlayer != null && audioPlayer.IsPlaying;

        private ListView tracksListBox;
        private Button playButton;
        private Button stopButton;
        private Slider volumeSlider;
        private AudioPlayer audioPlayer;
        private bool bFirstTime = true;

        public FrostySoundDataEditor(ILogger inLogger) 
            : base(inLogger)
        {
        }

        static FrostySoundDataEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrostySoundDataEditor), new FrameworkPropertyMetadata(typeof(FrostySoundDataEditor)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            tracksListBox = GetTemplateChild(PART_TracksListBox) as ListView;

            playButton = GetTemplateChild(PART_PlayButton) as Button;
            playButton.Click += PlayButton_Click;

            stopButton = GetTemplateChild(PART_StopButton) as Button;
            stopButton.Click += StopButton_Click;

            volumeSlider = GetTemplateChild(PART_VolumeSlider) as Slider;

            volumeSlider.Value = Math.Min(Config.Get<float>("SoundVolume", 20.0f), 100);

            volumeSlider.ValueChanged += VolumeSlider_ValueChanged;

            tracksListBox.SelectionChanged += TracksListBox_SelectionChanged;
            MenuItem mi = GetTemplateChild(PART_SoundExportMenuItem) as MenuItem;
            mi.Click += SoundExportMenuItem_Click;
            mi = GetTemplateChild(PART_SoundImportMenuItem) as MenuItem;
            mi.Click += SoundImportMenuItem_Click;
            Loaded += FrostySoundDataEditor_Loaded;

            TracksList = new ObservableCollection<SoundDataTrack>();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.SoundDispose();
            stopButton.IsEnabled = false;

            if (tracksListBox.SelectedItem != null)
                playButton.IsEnabled = true;
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(tracksListBox.SelectedItem is SoundDataTrack currentTrack))
                return;

            audioPlayer.OutputVoice.SetVolume((float)(volumeSlider.Value / 100.0));
            audioPlayer.PlaySound(currentTrack);

            playButton.IsEnabled = false;
            stopButton.IsEnabled = true;

            await Dispatcher.InvokeAsync(async () =>
            {
                while (IsPlaying)
                {
                    currentTrack.Progress = audioPlayer.Progress * 800.0;
                    await Task.Delay(30);
                }

                currentTrack.Progress = 0;
                stopButton.IsEnabled = false;
                playButton.IsEnabled = true;
            });
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider && slider.Value <= 100.0 && slider.Value >= 0.0)
            {
                audioPlayer.OutputVoice.SetVolume((float)(slider.Value / 100.0));

                Config.Add("SoundVolume", (float)slider.Value);
                Config.Save();
            }
        }

        private void TracksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tracksListBox.SelectedItem == null)
                return;

            if (!IsPlaying)
                playButton.IsEnabled = true;
        }

        public override void Closed()
        {
            audioPlayer.Dispose();
        }

        private void FrostySoundDataEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (bFirstTime)
            {
                audioPlayer = new AudioPlayer();

                List<SoundDataTrack> tracks = null;
                FrostyTaskWindow.Show("Loading tracks", "", owner =>
                {
                    tracks = InitialLoad(owner);
                });

                foreach (var track in tracks)
                    TracksList.Add(track);

                bFirstTime = false;
            }
        }

        protected virtual List<SoundDataTrack> InitialLoad(FrostyTaskWindow task)
        {
            return new List<SoundDataTrack>();
        }

        private void SoundExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!(tracksListBox.SelectedItem is SoundDataTrack track))
                return;

            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Save WAV File", "WAV file (*.wav)|*.wav", "Sound", AssetEntry.Filename);

            if (!sfd.ShowDialog())
                return;

            for (int trackIndex = 0; trackIndex < tracksListBox.SelectedItems.Count; trackIndex++)
            {
                SoundDataTrack indexedTrack = (SoundDataTrack) tracksListBox.SelectedItems[trackIndex];
                String indexedFilename = sfd.FileName.Replace(".wav", " " + trackIndex + ".wav");
                SoundExportMenuItem_Export(indexedTrack, indexedFilename);
                logger.Log("Exported {0} to {1}", AssetEntry.Name, indexedFilename);
            }
        }

        private void SoundExportMenuItem_Export(SoundDataTrack track, String filename)
        {
            FrostyTaskWindow.Show("Exporting Sound", "", task =>
            {
                WAV.WAVFormatChunk fmt = new WAV.WAVFormatChunk(WAV.WAVFormatChunk.DataFormats.WAVE_FORMAT_PCM, (ushort)track.ChannelCount, (uint)track.SampleRate, (uint)(track.ChannelCount * 2 * track.SampleRate), (ushort)(2 * track.ChannelCount), 16);
                List<WAV.WAVDataFrame> frames = new List<WAV.WAVDataFrame>();

                for (int i = 0; i < track.Samples.Length / track.ChannelCount; i++)
                {
                    // write frame
                    WAV.WAV16BitDataFrame frame = new WAV.WAV16BitDataFrame((ushort)track.ChannelCount);
                    for (int channel = 0; channel < track.ChannelCount; channel++)
                    {
                        frame.Data[channel] = track.Samples[i * track.ChannelCount + channel];
                    }
                    frames.Add(frame);
                }

                WAV.WAVDataChunk data = new WAV.WAVDataChunk(fmt, frames);
                WAV.RIFFMainChunk main = new WAV.RIFFMainChunk(new WAV.RIFFChunkHeader(0, new byte[] { 0x52, 0x49, 0x46, 0x46 }, 0), new byte[] { 0x57, 0x41, 0x56, 0x45 });

                using (FileStream stream = new FileStream(filename, FileMode.Create))
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    main.Write(writer, new List<WAV.IRIFFChunk>(new WAV.IRIFFChunk[] { fmt, data }));
                }
            });
        }

        private void SoundImportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (tracksListBox.SelectedItem == null)
                return;

            FrostyOpenFileDialog ofd = new FrostyOpenFileDialog("Import Sound", "Audio Files (*.mp3; *.wav)|*.mp3; *.wav", "Sound");
            if (ofd.ShowDialog())
            {
                try
                {
                    FrostyTaskWindow.Show("Importing track", "", (task) =>
                    {
                        ImportSound(ofd, task);
                    });
                }
                catch (Exception exp)
                {
                    App.AssetManager.RevertAsset(AssetEntry);
                    logger.LogError(exp.Message);
                }
            }
        }

        private void ImportSound(FrostyOpenFileDialog ofd, FrostyTaskWindow task)
        {
            //WaveFormat waveFormat = null;
            MemoryStream ms = new MemoryStream();

            if (ofd.FileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            {
                // force stereo for .wav files if needed
                using (var reader = new AudioFileReader(ofd.FileName))
                {
                    //waveFormat = reader.WaveFormat;

                    if (reader.WaveFormat.Channels == 1)
                    {
                        var stereo = new MonoToStereoSampleProvider(reader) { LeftVolume = 1.0f, RightVolume = 1.0f };
                        //waveFormat = stereo.WaveFormat;
                        WaveFileWriter.WriteWavFileToStream(ms, new SampleToWaveProvider16(stereo));
                    }
                    else
                    {
                        WaveFileWriter.WriteWavFileToStream(ms, reader);
                    }
                }
            }
            else if (ofd.FileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            {
                using (var reader = new MediaFoundationReader(ofd.FileName))
                {
                    //waveFormat = reader.WaveFormat;
                    WaveFileWriter.WriteWavFileToStream(ms, reader);
                }
            }

            byte[] resultBuf;
            using (var reader = new StreamMediaFoundationReader(ms))
            {
                int totalSamples = 0;
                using (var writer = new NativeWriter(new MemoryStream()))
                {
                    writer.Write(0x4800000c, Endian.Big);
                    writer.Write((byte)0x12); // codec, Pcm16Big
                    writer.Write((byte)((reader.WaveFormat.Channels - 1) << 2));
                    writer.Write((ushort)(reader.WaveFormat.SampleRate), Endian.Big);

                    long pos = writer.Position;
                    writer.Write(0x40000000, Endian.Big);

                    while (reader.Position < reader.Length)
                    {
                        int bufLength = 0x2600 * 2 * reader.WaveFormat.Channels;
                        if (totalSamples + 0x2600 > 0x00ffffff)
                            break;

                        byte[] buf = new byte[bufLength];

                        int actualRead = reader.Read(buf, 0, bufLength);
                        if (actualRead == 0)
                            break;

                        writer.Write((actualRead + 8) | 0x44000000, Endian.Big);
                        writer.Write(((actualRead / reader.WaveFormat.Channels) / 2), Endian.Big);

                        for (int i = 0; i < actualRead / 2; i++)
                        {
                            short s = BitConverter.ToInt16(buf, i * 2);
                            writer.Write(s, Endian.Big);
                        }

                        totalSamples += ((actualRead / reader.WaveFormat.Channels) / 2);
                    }

                    writer.Write(0x45000004, Endian.Big);
                    writer.Position = pos;
                    writer.Write(totalSamples | 0x40000000, Endian.Big);

                    resultBuf = writer.ToByteArray();
                }
            }

            int index = 0;
            Dispatcher?.Invoke(() => { index = tracksListBox.SelectedIndex; });

            dynamic soundWave = RootObject;
            dynamic variation = soundWave.RuntimeVariations[index];
            dynamic soundDataChunk = soundWave.Chunks[variation.ChunkIndex];
            ChunkAssetEntry chunkEntry = App.AssetManager.GetChunkEntry(soundDataChunk.ChunkId);

            int[] variationsPerChunk = new int[(int)soundWave.Chunks.Count];
            foreach (var rtVariation in soundWave.RuntimeVariations)
            {
                variationsPerChunk[rtVariation.ChunkIndex]++;
            }

            //bool modify = true;
            //if (modify)
            //{
            if (variationsPerChunk[variation.ChunkIndex] > 1)
            {
                using (NativeReader chunkReader = new NativeReader(App.AssetManager.GetChunk(chunkEntry)))
                {
                    IEnumerable<byte> buf;
                    if (variation.SegmentCount == soundWave.Segments.Count) // variation contains the only segments
                    {
                        buf = chunkReader.ReadToEnd();
                    }
                    else
                    {
                        buf = variation.FirstSegmentIndex != 0 ? chunkReader.ReadBytes((int)soundWave.Segments[variation.FirstSegmentIndex].SamplesOffset).Concat(resultBuf) : resultBuf;

                        if (variation.FirstSegmentIndex + variation.SegmentCount < soundWave.Segments.Count) // variation does not contain the final segments
                        {
                            chunkReader.Position = soundWave.Segments[variation.FirstSegmentIndex + variation.SegmentCount].SamplesOffset;
                            buf = buf.Concat(chunkReader.ReadToEnd()); // append the rest of the data

                            int sizeDiff = resultBuf.Length - ((int)soundWave.Segments[variation.FirstSegmentIndex + variation.SegmentCount].SamplesOffset - (int)soundWave.Segments[variation.FirstSegmentIndex].SamplesOffset);

                            for (int i = variation.FirstSegmentIndex + 1; i < soundWave.Segments.Count; i++)
                            {
                                if (soundWave.Segments[i].SamplesOffset == 0)
                                    break;

                                soundWave.Segments[i].SamplesOffset += sizeDiff;
                            }
                        }
                    }

                    resultBuf = buf.ToArray();
                }
            }

            App.AssetManager.ModifyChunk(chunkEntry.Id, resultBuf);
            soundDataChunk.ChunkSize = (uint)resultBuf.Length;

            // disable seekable data, not supported
            soundWave.Seekable = false;
            soundWave.Segments[variation.FirstSegmentIndex].SamplesOffset = 0;
            soundWave.Segments[variation.FirstSegmentIndex].SeekTableOffset = 4294967295;

            variation.SegmentCount = (byte)1;
            variation.FirstLoopSegmentIndex = (byte)0;
            variation.LastLoopSegmentIndex = (byte)0;

            //}
            //else // new chunk code
            //{
            //    Guid chunkId = App.AssetManager.AddChunk(resultBuf);

            //    ChunkAssetEntry newEntry = App.AssetManager.GetChunkEntry(chunkId);
            //    newEntry.AddToBundles(chunkEntry.Bundles);

            //    soundDataChunk = TypeLibrary.CreateObject("SoundDataChunk");
            //    soundDataChunk.ChunkId = chunkId;

            //    soundWave.Chunks.Add(soundDataChunk);

            //    dynamic segment = TypeLibrary.CreateObject("SoundWaveVariationSegment");
            //    segment.SeekTableOffset = 4294967295;
            //    soundWave.Segments.Add(segment);

            //    dynamic variation = TypeLibrary.CreateObject("SoundWaveRuntimeVariation");
            //    variation.FirstSegmentIndex = (ushort)(soundWave.Segments.Count - 1);
            //    variation.SegmentCount = (byte)1;
            //    variation.ChunkIndex = (byte)(soundWave.Chunks.Count - 1);
            //    variation.Weight = (byte)100;
            //    soundWave.RuntimeVariations.Add(variation);
            //}

            audioPlayer.Dispose();
            audioPlayer = new AudioPlayer();

            List<SoundDataTrack> tracks = InitialLoad(task);

            Dispatcher?.Invoke(() =>
            {
                // mark asset as modified and link the chunk
                AssetModified = true;
                InvokeOnAssetModified();
                EbxAssetEntry assetEntry = AssetEntry as EbxAssetEntry;
                assetEntry.LinkAsset(chunkEntry);

                TracksList.Clear();
                foreach (var track in tracks)
                    TracksList.Add(track);
            });
        }
    }
}

using FrostySdk;
using FrostySdk.Interfaces;
using FrostySdk.IO;
using FrostySdk.Managers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Frosty.Core.Controls;
using Frosty.Core.Windows;
using Frosty.Core;

namespace LegacyBigFilePlugin
{
    public class BigFileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is LegacyBigFileEditor.BigFileEntry bfe))
                return null;

            byte[] data = bfe.Data;
            if (data == null)
                return null;

            string param = (string)parameter;
            if (param == "TXT")
            {
                //if (data[0] == 0x49 && data[1] == 0x42 && data[2] == 0x58)
                //{
                //    StringBuilder sb = new StringBuilder();
                //    using (IBXReader reader = new IBXReader(new MemoryStream(data)))
                //        sb.Append(reader.ReadXml());

                //    return sb.ToString();
                //}
                //else
                {
                    string name = bfe.Name;
                    StringBuilder sb = new StringBuilder();

                    if (name.EndsWith(".xml") || name.EndsWith(".lua") || (bfe.Data[0] == 0x3C && bfe.Data[1] == 0x53 && bfe.Data[2] == 0x43))
                    {
                        sb.Append(Encoding.UTF8.GetString(data));
                    }
                    else
                    {
                        string line1 = "";
                        string line2 = "";

                        for (int i = 0; i < data.Length; i++)
                        {
                            if (i != 0 && i % 16 == 0)
                            {
                                sb.Append(line1 + line2 + "\r\n");
                                line1 = "";
                                line2 = "";
                            }

                            line1 += data[i].ToString("X2") + " ";
                            line2 += (char.IsLetterOrDigit((char)data[i]) || char.IsSymbol((char)data[i])) ? (char)data[i] : '.';
                        }

                        sb.Append(line1.PadRight(48) + line2 + "\r\n");
                    }

                    return sb.ToString();
                }
            }

            return bfe.Data;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IBXReader : NativeReader
    {
        private class PropertyValueInfo
        {
            public string TypeName
            {
                get
                {
                    switch (Type)
                    {
                        case 0xE0: return "string";
                        case 0xD0: return "string";
                        case 0xC0: return "string";
                        case 0xB0: return "float";
                        case 0x00: return "int";
                        case 0x10: return "int";
                        case 0x20: return "int";
                        case 0x30: return "int";
                        case 0x40: return "bool";
                        default: return "0x" + Type.ToString("X2");
                    }
                }
            }
            public string DataString
            {
                get
                {
                    switch (Type)
                    {
                        case 0xE0: return Encoding.UTF8.GetString(Data);
                        case 0xD0: return Encoding.UTF8.GetString(Data);
                        case 0xC0: return Encoding.UTF8.GetString(Data);
                        case 0xB0: return BitConverter.ToSingle(Data, 0).ToString();
                        case 0x00: return Data[0].ToString();
                        case 0x10: return Data[0].ToString();
                        case 0x20: return BitConverter.ToInt16(Data, 0).ToString();
                        case 0x30: return BitConverter.ToInt32(Data, 0).ToString();
                        case 0x40: return (Data[0] == 1) ? "True" : "False";
                        default: return BitConverter.ToString(Data);
                    }
                }
            }
            public int Type { get; set; }
            public byte[] Data { get; set; }
        }
        private class PropertyInfo
        {
            public string Name { get; set; }
            public PropertyValueInfo Value { get; set; }
        }
        private class ClassInfo
        {
            public string Name { get; set; }
            public List<PropertyInfo> Properties { get; set; } = new List<PropertyInfo>();
            public List<ClassInfo> Children { get; set; } = new List<ClassInfo>();
        }

        private List<string> strings = new List<string>();
        private List<PropertyValueInfo> values = new List<PropertyValueInfo>();

        public IBXReader(Stream inStream)
            : base(inStream)
        {
        }

        public string ReadXml()
        {
            uint magic = ReadUInt();
            int numStrings = ReadCompressedInt();

            for (int i = 0; i < numStrings; i++)
            {
                int len = ReadCompressedInt() + 1;
                string str = SanitizeString(ReadSizedString(len));
                strings.Add(str);
            }

            int numProps = ReadCompressedInt();
            for (int i = 0; i < numProps; i++)
            {
                byte b = ReadByte();
                byte type = (byte)(b & 0xF0);

                byte[] val = null;

                switch (type)
                {
                    case 0xE0: val = Encoding.UTF8.GetBytes(strings[ReadUShort(Endian.Big)]); break;
                    case 0xD0: val = Encoding.UTF8.GetBytes(strings[ReadByte()]); break;
                    case 0xC0: val = Encoding.UTF8.GetBytes(strings[(b & 0x3F)]); break;
                    case 0xB0: val = ReadBytes(4); break;
                    case 0x40: val = new byte[] { (byte)(b & 0x0F) }; break;
                    case 0x30: val = ReadBytes(4); break;
                    case 0x20: val = ReadBytes(2); break;
                    case 0x10: val = ReadBytes(1); break;
                    case 0x00: val = new byte[] { b }; break;
                    default: val = new byte[] { b }; break;
                }

                values.Add(new PropertyValueInfo()
                {
                    Type = type,
                    Data = val
                });
            }

            ReadByte();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"us-ascii\"?>");
            sb.Append(PrintClass(ProcessClass()));
            return sb.ToString();
        }

        private ClassInfo ProcessClass()
        {
            byte b1 = ReadByte();
            int stringId = ReadCompressedInt();
            string name = strings[stringId];
            int numElems = ReadCompressedInt();
            int numChildren = ReadCompressedInt();

            ClassInfo ci = new ClassInfo() { Name = name };
            for (int i = 0; i < numElems; i++)
            {
                byte b = ReadByte();
                int propId = b & 0x7F;

                if ((b & 0xF0) == 0xC0) { propId = ReadUShort(Endian.Big); }
                else if ((b & 0xF0) == 0xA0) { propId = ReadByte(); }
                else if ((b & 0xF0) > 0xC0) { }

                int valueId = ReadCompressedInt();

                ci.Properties.Add(new PropertyInfo()
                {
                    Name = strings[propId],
                    Value = values[valueId]
                });
            }

            for (int i = 0; i < numChildren; i++)
                ci.Children.Add(ProcessClass());

            return ci;
        }

        private string PrintClass(ClassInfo ci, int tabs = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("".PadLeft(tabs) + "<" + ci.Name);
            if (ci.Properties.Count > 0 || ci.Children.Count > 0)
            {
                sb.AppendLine(">");

                if (ci.Properties.Count > 0)
                {
                    tabs += 2;
                    foreach (PropertyInfo pi in ci.Properties)
                    {
                        sb.AppendLine("".PadLeft(tabs) + "<property name=\"" + pi.Name + "\" type=\"" + pi.Value.TypeName + "\" value=\"" + pi.Value.DataString + "\"/>");
                    }
                    tabs -= 2;
                }
                if (ci.Children.Count > 0)
                {
                    tabs += 2;
                    foreach (ClassInfo child in ci.Children)
                        sb.Append(PrintClass(child, tabs));
                    tabs -= 2;
                }

                sb.AppendLine("".PadLeft(tabs) + "<" + ci.Name + "/>");
            }
            else
                sb.AppendLine("/>");
            return sb.ToString();
        }

        private int ReadCompressedInt()
        {
            byte val = ReadByte();
            int outVal = 0;
            if ((val & 0x80) != 0)
                outVal = ReadUShort(Endian.Big) + (val & 0x40);
            else if ((val & 0x40) != 0)
                outVal = ReadByte();
            outVal += (val & 0x3F);
            return outVal;
        }

        private string SanitizeString(string inStr)
        {
            inStr = inStr.Replace("\r", "\\r");
            inStr = inStr.Replace("\n", "\\n");
            return inStr;
        }
    }

    [TemplatePart(Name = PART_TextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_ListBox, Type = typeof(ListBox))]
    [TemplatePart(Name = PART_ExportButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_ImportButton, Type = typeof(Button))]
    public class LegacyBigFileEditor : FrostyAssetEditor
    {
        public class BigFileEntry
        {
            public string DisplayName => Name + " (" + Size + " bytes)";

            public string Name { get; set; }
            public byte[] Hash { get; set; }
            public uint Offset { get; set; }
            public uint Size { get; set; }
            public byte[] Data { get; set; }
            public string Type { get; set; }

            public BigFileEntry(string inName, uint inOffset, uint inSize, byte[] inData, string inType="MISC")
            {
                Name = inName;
                Offset = inOffset;
                Size = inSize;
                Data = inData;
                Type = inType;
            }
        }

        private const string PART_TextBox = "PART_TextBox";
        private const string PART_ListBox = "PART_ListBox";
        private const string PART_ExportButton = "PART_ExportButton";
        private const string PART_ImportButton = "PART_ImportButton";

        private ListBox lb;
        private Button exportButton;
        private Button importButton;

        private bool firstTimeLoad = true;

        static LegacyBigFileEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LegacyBigFileEditor), new FrameworkPropertyMetadata(typeof(LegacyBigFileEditor)));
        }

        public LegacyBigFileEditor(ILogger inLogger)
            : base(inLogger)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            lb = GetTemplateChild(PART_ListBox) as ListBox;
            exportButton = GetTemplateChild(PART_ExportButton) as Button;
            importButton = GetTemplateChild(PART_ImportButton) as Button;

            Loaded += LegacyTextEditor_Loaded;
            lb.SelectionChanged += Lb_SelectionChanged;
            exportButton.Click += ExportButton_Click;
            importButton.Click += ImportButton_Click;
        }

        public override List<ToolbarItem> RegisterToolbarItems()
        {
            return new List<ToolbarItem>();
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            BigFileEntry bfe = lb.SelectedItem as BigFileEntry;
            if (bfe == null)
                return;

            AssetEntry asset = AssetEntry;

            string filter = bfe.Type + " (*." + bfe.Type + ")|*." + bfe.Type;
            FrostyOpenFileDialog ofd = new FrostyOpenFileDialog("Import File", filter, "BigFile");
            if (ofd.ShowDialog())
            {
                FrostyTaskWindow.Show("Importing File", "", (task) =>
                {
                    byte[] buffer = null;
                    using (NativeReader reader = new NativeReader(new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read)))
                        buffer = reader.ReadToEnd();
                    bfe.Data = buffer;
                    bfe.Size = (uint)buffer.Length;
                    Reconstruct(asset);
                });

                //LoadSelectedAsset();
                lb.Items.Refresh();

                InvokeOnAssetModified();
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            BigFileEntry bfe = lb.SelectedItem as BigFileEntry;
            if (bfe == null)
                return;

            string filter = bfe.Type + " (*." + bfe.Type + ")|*." + bfe.Type;
            FrostySaveFileDialog sfd = new FrostySaveFileDialog("Export File", filter, "BigFile", "", false);
            if (sfd.ShowDialog())
            {
                FrostyTaskWindow.Show("Exporting File", "", (task) =>
                {
                    //if (bfe.Data[0] == 0x49 && bfe.Data[1] == 0x42 && bfe.Data[2] == 0x58)
                    //{
                    //    using (IBXReader reader = new IBXReader(new MemoryStream(bfe.Data)))
                    //    {
                    //        using (NativeWriter writer = new NativeWriter(new FileStream(sfd.FileName, FileMode.Create)))
                    //            writer.WriteLine(reader.ReadXml());
                    //    }
                    //}
                    //else
                    {
                        using (NativeWriter writer = new NativeWriter(new FileStream(sfd.FileName, FileMode.Create)))
                            writer.Write(bfe.Data);
                    }
                });
                logger.Log("File exported to " + sfd.FileName);
            }
        }

        private void Lb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //LoadSelectedAsset();
        }

        private void LegacyTextEditor_Loaded(object sender, RoutedEventArgs e)
        {
            if (firstTimeLoad)
            {
                if (AssetEntry.Type == "BIG")
                {
                    using (NativeReader reader = new NativeReader(App.AssetManager.GetCustomAsset("legacy", AssetEntry)))
                    {
                        uint magic = reader.ReadUInt();
                        bool isCompressed = magic == 0x34474942;

                        if (magic != 0x46474942 && magic != 0x34474942)
                        {
                            return;
                        }

                        uint totalDataSize = reader.ReadUInt();
                        uint numFiles = reader.ReadUInt(Endian.Big);
                        uint unkOffset = reader.ReadUInt(Endian.Big);

                        List<BigFileEntry> entries = new List<BigFileEntry>();
                        string type = "DAT";

                        for (uint i = 0; i < numFiles; i++)
                        {
                            uint offset = reader.ReadUInt(Endian.Big);
                            uint size = reader.ReadUInt(Endian.Big);
                            string name = reader.ReadNullTerminatedString();

                            if (size == 0)
                            {
                                if (name == "sg1") type = "DDS";
                                else if (name == "sg2") type = "APT";
                                continue;
                            }

                            long pos = reader.Position;
                            reader.Position = offset;
                            byte[] buf= reader.ReadBytes((int)size);
                            if (isCompressed)
                                buf = DecompressEAHD(buf);
                            reader.Position = pos;

                            entries.Add(new BigFileEntry(name, offset, size, buf, type));
                        }

                        lb.ItemsSource = entries;
                    }
                }
                else
                {
                    using (NativeReader reader = new NativeReader(App.AssetManager.GetCustomAsset("legacy", AssetEntry)))
                    {
                        uint magic = reader.ReadUInt();
                        if (magic != 0x41464742)
                        {
                            return;
                        }

                        uint version = reader.ReadUInt();
                        uint nextFileIndex = reader.ReadUInt();
                        uint numFiles = reader.ReadUInt();
                        uint tocOffset = reader.ReadUInt();
                        uint unk1 = reader.ReadUInt();
                        uint dataOffset = reader.ReadUInt();
                        reader.ReadUInt();
                        byte[] sizes = reader.ReadBytes(6);
                        ushort unk2 = reader.ReadUShort();
                        uint numUnknowns = reader.ReadUInt();
                        reader.Position += 0x14;

                        List<uint> unknowns = new List<uint>();
                        for (uint i = 0; i < numUnknowns; i++)
                            unknowns.Add(reader.ReadUInt());

                        List<BigFileEntry> entries = new List<BigFileEntry>();
                        for (uint i = 0; i < numFiles; i++)
                        {
                            uint isCompressed = ReadVarInt(reader, sizes[0]);
                            int index = (int)ReadVarInt(reader, sizes[1]);
                            byte[] hash = reader.ReadBytes(sizes[2]);
                            uint offset = ReadVarInt(reader, sizes[3]) * 8;
                            uint compressedSize = ReadVarInt(reader, sizes[4]);
                            uint uncompressedSize = ReadVarInt(reader, sizes[5]) + compressedSize;

                            long pos = reader.Position;
                            reader.Position = offset;
                            byte[] buf = reader.ReadBytes((int)compressedSize);
                            if (isCompressed != 0)
                                buf = Utils.DecompressZLib(buf, (int)uncompressedSize);
                            reader.Position = pos;

                            string type = "";
                            switch(unknowns[index])
                            {
                                case 0x264923D3: type = "DDS"; break;
                                case 0x264923D2: type = "UNK"; break;
                                case 0x264923D6: type = "APT"; break;
                                case 0x264923D5: type = "APTD"; break;
                                case 0x8fe5b6d9: type = "DAT"; break;
                                default: type = "DAT"; break;
                            }
                            entries.Add(new BigFileEntry(BitConverter.ToString(hash), offset, compressedSize, buf, type) { Hash = hash });
                        }

                        entries.Sort((BigFileEntry a, BigFileEntry b) => a.Offset.CompareTo(b.Offset));
                        lb.ItemsSource = entries;
                    }
                }
            }
        }

        private byte[] DecompressEAHD(byte[] buffer)
        {
            using (NativeReader reader = new NativeReader(new MemoryStream(buffer)))
            {
                ushort magic = reader.ReadUShort();
                if (magic != 0xfb10)
                    return buffer;

                uint totalSize = ReadVarInt(reader, 3, Endian.Big);
                byte[] outBuf = new byte[totalSize];
                int pos1 = 0;
                
                while (reader.Position < reader.Length)
                {
                    byte ctrl = reader.ReadByte();

                    int numBytesToRead = 0;
                    int numBytesToCopy = 0;
                    int copyOffset = 0;

                    // 0x00 - 0x7F
                    if (ctrl < 0x80)
                    {
                        int a = reader.ReadByte();
                        numBytesToRead = ctrl & 0x03;
                        numBytesToCopy = ((ctrl & 0x1C) >> 2) + 3;
                        copyOffset = ((ctrl & 0x60) << 3) + a + 1;
                    }

                    // 0x80 - 0xBF
                    else if (ctrl < 0xC0)
                    {
                        int a = reader.ReadByte();
                        int b = reader.ReadByte();

                        numBytesToRead = ((a & 0xC0) >> 6) & 0x03;
                        numBytesToCopy = (ctrl & 0x3F) + 4;
                        copyOffset = ((a & 0x3F) << 8) + b + 1;
                    }

                    // 0xC0 - 0xDF
                    else if (ctrl < 0xE0)
                    {
                        int a = reader.ReadByte();
                        int b = reader.ReadByte();
                        int c = reader.ReadByte();

                        numBytesToRead = ctrl & 0x03;
                        numBytesToCopy = ((ctrl & 0x0C) << 6) + c + 5;
                        copyOffset = ((ctrl & 0x10) << 12) + (a << 8) + b + 1;

                    }

                    // 0xE0 - 0xFB
                    else if (ctrl < 0xFC)
                    {
                        numBytesToRead = ((ctrl & 0x1F) << 2) + 4;
                    }

                    // 0xFC - 0xFF
                    else
                    {
                        numBytesToRead = ctrl & 0x03;
                    }

                    for (int i = 0; i < numBytesToRead; i++)
                        outBuf[pos1++] = reader.ReadByte();
                    copyOffset = pos1 - copyOffset;
                    for (int i = 0; i < numBytesToCopy; i++)
                        outBuf[pos1++] = outBuf[copyOffset + i];
                }

                return outBuf;
            }
        }

        private uint ReadVarInt(NativeReader reader, int size, Endian endian = Endian.Little)
        {
            byte[] buf = reader.ReadBytes(size);
            uint value = 0;

            if (endian == Endian.Little)
            {
                for (int i = 0; i < size; i++)
                    value |= ((uint)buf[i] << (i * 8));
            }
            else
            {
                for (int i = 0; i < size; i++)
                    value |= ((uint)buf[i] << ((size - i - 1) * 8));
            }

            return value;
        }

        private void WriteVarInt(NativeWriter writer, int size, uint value)
        {
            byte[] buf = new byte[size];
            for (int i = 0; i < size; i++)
                buf[i] = (byte)(value >> (i * 8));
            writer.Write(buf);
        }

        private void Reconstruct(AssetEntry assetEntry)
        {
            if (assetEntry.Type == "BIG") ReconstructBIG(assetEntry);
            else ReconstructAST(assetEntry);
        }

        private void ReconstructBIG(AssetEntry assetEntry)
        {
            MemoryStream ms = new MemoryStream();
            List<uint> offsets = new List<uint>();
            uint tocSize = 0;

            using (NativeWriter writer = new NativeWriter(ms, true))
            {
                string lastType = "DAT";
                foreach (BigFileEntry bfe in lb.Items)
                {
                    offsets.Add((uint)writer.Position);
                    writer.Write(bfe.Data);
                    while (writer.Position % 0x40 != 0)
                        writer.Position++;

                    tocSize += (uint)(8 + bfe.Name.Length + 1);
                    if (bfe.Type != lastType)
                    {
                        lastType = bfe.Type;
                        tocSize += 12;
                    }
                }
            }

            byte[] data = ms.ToArray();
            ms.Dispose();

            using (NativeReader reader = new NativeReader(App.AssetManager.GetCustomAsset("legacy", assetEntry)))
            {
                using (NativeWriter writer = new NativeWriter(new MemoryStream()))
                {
                    uint magic = reader.ReadUInt();
                    uint totalDataSize = reader.ReadUInt();
                    uint numFiles = reader.ReadUInt(Endian.Big);
                    uint unkOffset = reader.ReadUInt(Endian.Big);

                    totalDataSize = tocSize + 8;
                    while ((totalDataSize + 16) % 0x40 != 0)
                        totalDataSize++;

                    uint startDataPos = totalDataSize + 16;
                    totalDataSize += (uint)data.Length;

                    writer.Write(magic);
                    writer.Write(totalDataSize);
                    writer.Write(numFiles, Endian.Big);
                    writer.Write(tocSize + 16 + 8, Endian.Big);

                    int index = 0;
                    string lastType = "DAT";
                    foreach (BigFileEntry bfe in lb.Items)
                    {
                        if (bfe.Type != lastType)
                        {
                            lastType = bfe.Type;
                            string gName = "";

                            if (lastType == "DDS") gName = "sg1";
                            else if (lastType == "APT") gName = "sg2";

                            writer.Write(offsets[index] + startDataPos, Endian.Big);
                            writer.Write(0);
                            writer.WriteNullTerminatedString(gName);
                        }

                        writer.Write(offsets[index++] + startDataPos, Endian.Big);
                        writer.Write(bfe.Data.Length, Endian.Big);
                        writer.WriteNullTerminatedString(bfe.Name);
                    }

                    writer.Write(new byte[] { 0x4C, 0x32, 0x38, 0x36, 0x15, 0x05, 0x00, 0x00 });
                    while (writer.Position % 0x40 != 0)
                        writer.Write((byte)0x00);
                    
                    writer.Write(ms.ToArray());
                    
                    App.AssetManager.ModifyCustomAsset("legacy", assetEntry.Name, writer.ToByteArray());
                }
            }
        }

        private void ReconstructAST(AssetEntry assetEntry)
        {
            MemoryStream ms = new MemoryStream();
            List<uint> offsets = new List<uint>();

            using (NativeWriter writer = new NativeWriter(ms, true))
            {
                foreach (BigFileEntry bfe in lb.Items)
                {
                    offsets.Add((uint)writer.Position);
                    writer.Write(bfe.Data);
                    while (writer.Position % 8 != 0)
                        writer.Position++;
                }
            }

            using (NativeReader reader = new NativeReader(App.AssetManager.GetCustomAsset("legacy", assetEntry)))
            {
                using (NativeWriter writer = new NativeWriter(new MemoryStream()))
                {
                    uint magic = reader.ReadUInt();
                    uint version = reader.ReadUInt();
                    uint nextFileIndex = reader.ReadUInt();
                    uint numFiles = reader.ReadUInt();
                    uint tocOffset = reader.ReadUInt();
                    uint unk1 = reader.ReadUInt();
                    uint tocSize = reader.ReadUInt();
                    uint unk2 = reader.ReadUInt();
                    byte[] sizes = reader.ReadBytes(6);
                    ushort unk3 = reader.ReadUShort();
                    uint numTypes = reader.ReadUInt();
                    reader.Position += 0x14;

                    List<uint> types = new List<uint>();
                    for (uint i = 0; i < numTypes; i++)
                        types.Add(reader.ReadUInt());

                    sizes[3] = 4;
                    sizes[4] = 4;
                    sizes[5] = 4;

                    tocSize = (uint)(((sizes[0] + sizes[1] + sizes[2] + sizes[3] + sizes[4] + sizes[5]) * lb.Items.Count) + (numTypes * 4));

                    writer.Write(magic);
                    writer.Write(version);
                    writer.Write(lb.Items.Count + 1);
                    writer.Write(lb.Items.Count);
                    writer.Write(tocOffset);
                    writer.Write(unk1);
                    writer.Write(tocSize);
                    writer.Write(unk2);
                    writer.Write(sizes);
                    writer.Write(unk3);
                    writer.Write(numTypes);
                    for (int i = 0; i < 0x14 / 4; i++)
                        writer.Write(0x00);

                    for (int i = 0; i < numTypes; i++)
                        writer.Write(types[i]);

                    int index = 0;
                    uint actualOffset = tocOffset + tocSize;
                    while (actualOffset % 8 != 0)
                        actualOffset++;

                    foreach (BigFileEntry bfe in lb.Items)
                    {
                        uint typeIndex = 0;
                        switch (bfe.Type)
                        {
                            case "DDS": typeIndex = 0x264923D3; break;
                            case "UNK": typeIndex = 0x264923D2; break;
                            case "APT": typeIndex = 0x264923D6; break;
                            case "APTD": typeIndex = 0x264923D5; break;
                            case "DAT": typeIndex = 0x8fe5b6d9; break;
                        }

                        WriteVarInt(writer, sizes[0], 0);
                        WriteVarInt(writer, sizes[1], (uint)types.IndexOf(typeIndex));
                        writer.Write(bfe.Hash);
                        WriteVarInt(writer, sizes[3], (offsets[index++] + actualOffset) / 8);
                        WriteVarInt(writer, sizes[4], (uint)bfe.Data.Length);
                        WriteVarInt(writer, sizes[5], 0);
                    }

                    while (writer.Position < actualOffset)
                        writer.Write((byte)0x00);

                    writer.Write(ms.ToArray());
                    App.AssetManager.ModifyCustomAsset("legacy", assetEntry.Name, writer.ToByteArray());
                }
            }

            ms.Dispose();
        }
    }
}

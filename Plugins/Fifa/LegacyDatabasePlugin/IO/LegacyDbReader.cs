using FrostySdk.IO;
using LegacyDatabasePlugin.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;

namespace LegacyDatabasePlugin.IO
{
    public class LegacyDbReader : NativeReader
    {
        private class LegacyDbColumnDescriptor
        {
            public LegacyDbColumn Column;
            public int BitOffset;
            public int Depth;

            public int RangeLow;
            public int RangeHigh;
        }

        private DataSet descriptor;
        public LegacyDbReader(Stream inMetaStream, Stream inDbStream)
            : base(inDbStream)
        {
            if (inMetaStream != null)
            {
                descriptor = new DataSet() { Locale = new CultureInfo("en-US") };
                descriptor.ReadXml(inMetaStream);
            }
        }

        public LegacyDb ReadDb()
        {
            uint magic = ReadUInt();
            if (magic != 0x08004244)
                return null;

            Endian endian = (Endian)ReadInt();
            uint totalDataLength = ReadUInt(endian);
            Position += 4;
            int tableCount = ReadInt(endian);
            uint crcHeader = ReadUInt(endian);

            LegacyDb db = new LegacyDb();
            List<uint> tableOffsets = new List<uint>();

            for (int i = 0; i < tableCount; i++)
            {
                string shortTableName = ReadShortName(endian);
                uint tableOffset = ReadUInt(endian);

                string tableName = shortTableName;
                if (descriptor != null)
                {
                    foreach (DataRow row in descriptor.Tables["table"].Rows)
                    {
                        string shortName = (string)row["shortName"];
                        if (shortName.Equals(shortTableName))
                        {
                            tableName = (string)row["name"];
                            break;
                        }
                    }
                }

                tableOffsets.Add(tableOffset);
                db.AddTable(new LegacyDbTable(shortTableName, tableName));
            }

            uint crcShortNames = ReadUInt();
            long startTablesPos = Position;

            for (int i = 0; i < tableOffsets.Count; i++)
            {
                Position = startTablesPos + tableOffsets[i];

                LegacyDbTable table = db[i];
                List<LegacyDbColumnDescriptor> columns = new List<LegacyDbColumnDescriptor>();

                int unk = ReadInt(endian);
                int recordSize = ReadInt(endian);
                uint bitRecordsCount = ReadUInt(endian);
                uint compressedStringLen = ReadUInt(endian);
                ushort recordCount = ReadUShort(endian);
                ushort writtenRecordCount = ReadUShort(endian);
                ushort cancelledRecordCount = ReadUShort(endian);
                ushort unk2 = ReadUShort(endian);
                uint fieldCount = ReadByte();
                Pad(4);
                uint unk3 = ReadUInt(endian);
                uint crcTableHeader = ReadUInt(endian);

                for (int j = 0; j < fieldCount; j++)
                {
                    int fieldType = ReadInt(endian);
                    int bitOffset = ReadInt(endian);
                    string fieldShortName = ReadShortName(endian);
                    int depth = ReadInt(endian);
                    int rangeLow = 0;
                    int rangeHigh = 0;
                    bool key = false;

                    string fieldName = fieldShortName;
                    if (descriptor != null)
                    {
                        foreach (DataRow row in descriptor.Tables["field"].Rows)
                        {
                            string shortName = (string)row["shortName"];
                            if (shortName.Equals(fieldShortName))
                            {
                                int fieldsId = (int)row["fields_id"];
                                int tableId = (int)descriptor.Tables["fields"].Rows[fieldsId]["table_id"];
                                string tableName = (string)descriptor.Tables["table"].Rows[tableId]["shortName"];

                                if (tableName.Equals(table.ShortName))
                                {
                                    fieldName = (string)row["name"];
                                    rangeLow = int.Parse((string)row["rangeLow"]);
                                    rangeHigh = int.Parse((string)row["rangeHigh"]);
                                    if (row.Table.Columns.Contains("key") && !(row["key"] is DBNull))
                                        key = ((string)row["key"]).Equals("True", StringComparison.OrdinalIgnoreCase);
                                    break;
                                }
                            }
                        }
                    }

                    LegacyDbColumn column = new LegacyDbColumn(table, (LegacyDbColumnType)fieldType, fieldShortName, fieldName, key);
                    columns.Add(new LegacyDbColumnDescriptor()
                    {
                        Column = column,
                        BitOffset = bitOffset,
                        Depth = depth,
                        RangeLow = rangeLow,
                        RangeHigh = rangeHigh
                    });
                    table.AddColumn(column);
                }

                List<List<object>> writtenValues = new List<List<object>>();
                int huffmanNodeCount = int.MaxValue;

                for (int j = 0; j < writtenRecordCount; j++)
                {
                    LegacyDbRow row = new LegacyDbRow(table);
                    long startRecordPos = Position;

                    for (int k = 0; k < columns.Count; k++)
                    {
                        LegacyDbColumnDescriptor colDesc = columns[k];
                        object value = null;

                        switch (colDesc.Column.Type)
                        {
                            case LegacyDbColumnType.String:
                                {
                                    Position = startRecordPos + (colDesc.BitOffset / 8);
                                    value = Encoding.UTF8.GetString(ReadBytes(colDesc.Depth / 8)).Trim('\0');
                                }
                                break;
                            case LegacyDbColumnType.Unk2:
                            case LegacyDbColumnType.Integer:
                                {
                                    SetBitPosition(startRecordPos, colDesc.BitOffset);
                                    value = colDesc.RangeLow + ReadBitInt(colDesc.Depth, endian);
                                }
                                break;
                            case LegacyDbColumnType.Float:
                                {
                                    Position = startRecordPos + (colDesc.BitOffset / 8);
                                    value = ReadFloat(endian);
                                }
                                break;
                            case LegacyDbColumnType.Unk1:
                                break;
                            default:
                                {
                                    Position = startRecordPos + (colDesc.BitOffset / 8);
                                    int val = ReadInt(endian);

                                    if (val != -1 && val < huffmanNodeCount)
                                        huffmanNodeCount = val;

                                    value = val;
                                }
                                break;
                        }

                        row.SetValue(colDesc.Column, value);
                    }

                    while (Position - startRecordPos < recordSize)
                        Position++;

                    table.AddRow(row);
                }

                long startStringsPos = Position;
                if (huffmanNodeCount != int.MaxValue)
                {
                    HuffmannTree tree = new HuffmannTree(huffmanNodeCount / 4);
                    tree.Load(this);

                    foreach (LegacyDbRow row in table.Rows)
                    {
                        foreach (LegacyDbColumnValue colValue in row.Values)
                        {
                            LegacyDbColumn column = colValue.Column;
                            if (column.Type >= LegacyDbColumnType.ShortCompressedString)
                            {
                                Position = startStringsPos + (int)(colValue.Value);

                                short value = 0;
                                if (column.Type == LegacyDbColumnType.ShortCompressedString)
                                {
                                    value = ReadByte();
                                }
                                else if (column.Type == LegacyDbColumnType.LongCompressedString)
                                {
                                    value = ReadShort(Endian.Big);
                                }

                                colValue.Value = tree.ReadString(this, value);
                            }
                        }
                    }
                }

                Position = startStringsPos + compressedStringLen;
                uint crcRecords = ReadUInt(endian);
            }

            return db;
        }

        private int bitPosition = 0;
        private byte bitBuffer;

        public void SetBitPosition(long absoluteOffset, long bitOffset)
        {
            Position = absoluteOffset;
            Position += bitOffset / 8;

            bitPosition = (int)(bitOffset % 8);
            if (bitPosition > 0)
                bitBuffer = ReadByte();
        }

        public int ReadBitInt(int length, Endian endian)
        {
            int retVal = 0;

            if (endian == Endian.Big)
            {
                if (bitPosition == 0)
                    bitBuffer = ReadByte();
                for (int i = length - 1; i >= 0; i--)
                {
                    int tmp = ((bitBuffer & 0x80 >> bitPosition) == 0) ? 0 : 1;
                    retVal += tmp << i;
                    bitPosition++;
                    if (bitPosition == 8)
                    {
                        bitBuffer = ReadByte();
                        bitPosition = 0;
                    }
                }
                return retVal;
            }
            else
            {
                int i = 0;
                if (bitPosition > 0)
                {
                    i = 8 - bitPosition;
                    retVal = bitBuffer >> bitPosition;
                }
                while (i < length)
                {
                    bitBuffer = ReadByte();
                    retVal += bitBuffer << i;
                    i += 8;
                }

                bitPosition = length + 8 - i & 7;
                retVal &= ((int)((1L << length) - 1L));
            }

            return retVal;
        }

        public string ReadShortName(Endian endian)
        {
            uint val = ReadUInt(endian);
            return Encoding.UTF8.GetString(BitConverter.GetBytes(val));
        }
    }
}

using System.Collections.Generic;
using System.Text;
using AnimationEditorPlugin.Formats.Sections;
using FrostySdk.IO;

namespace AnimationEditorPlugin.Formats
{
    public enum BankType
    {
        Invalid,
        Bool,
        Int8,
        Uint8,
        Int16,
        Uint16,
        Int32,
        UInt32,
        Int64,
        UInt64
    }

    public class Bank
    {
        public class Entry
        {
            public string Name;
            public int Type;
            public int Size;
            public int Position;
            
            public uint BankHash;
            public Bank Bank;
        }
        
        public string Name => m_name;
        
        private string m_name;
        private List<Entry> m_entries;
        private uint m_type;
        private int m_minEntryNum;
        private int m_maxEntryNum;
        private int m_size;
        private int m_alignment;
        
        private int m_nameTableSize;
        private byte[] m_nameTable;

        private Endian m_endian;
        
        public Bank(Endian endian)
        {
            m_endian = endian;
        }
        
        public override string ToString() => m_nameTable != null ? Name : "Empty";
        
        public void Read(NativeReader reader, Endian endian, long bankStartPosition, Dictionary<long, Bank> banks, int bankVersion = 1)
        {
            m_minEntryNum = reader.ReadInt(endian);
            m_maxEntryNum = reader.ReadInt(endian);
            m_size = reader.ReadInt(endian);
            m_alignment = reader.ReadInt(endian);

            // unknown
            reader.ReadUInt(endian);
            
            m_nameTableSize = reader.ReadInt(endian);

            // unknowns
            reader.ReadBoolean();
            reader.ReadBoolean();
            reader.ReadUShort(endian);

            m_type = reader.ReadUInt(endian);
            
            // entries
            int entryCount = (m_maxEntryNum - m_minEntryNum) + 1;
            for (int i = 0; i < entryCount; i++)
            {
                Bank.Entry entry = new Entry();
                entry.BankHash = reader.ReadUInt(endian);
                entry.Size = reader.ReadInt(endian);
                entry.Position = reader.ReadInt(endian);
                // unknowns
                reader.ReadInt(endian); // name
                reader.ReadUInt(endian); // count / flags
                reader.ReadUShort(endian); // element align
                reader.ReadShort(endian); // RLE

                if (bankVersion == 1)
                {
                    long position = reader.ReadLong(endian);
                    if (position != 0)
                    {
                        entry.Bank = banks[position];
                    }
                }
                else if (bankVersion == 2)
                {
                    Pointer pointer = new Pointer(reader.Position - bankStartPosition, reader.ReadLong(m_endian));
                    if (pointer.GetPosition() != 0)
                    {
                        entry.Bank = banks[pointer.GetPosition()];
                    }
                }
                
                entry.Type = m_minEntryNum + i;
            }
            
            // names
            m_nameTable = reader.ReadBytes(m_nameTableSize);

            m_name = GetName(1);
        }

        private string GetName(int offset)
        {
            int len = 0;
            while (m_nameTable[offset + len] != 0)
            {
                len++;
            }

            return Encoding.ASCII.GetString(m_nameTable, offset, len);
        }
    }
}
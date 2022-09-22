using System;
using System.Collections.Generic;
using FrostySdk.IO;

namespace AnimationEditorPlugin.Formats.Sections
{
    public class Section_DATA : Section
    {
        public int Size { get; protected set; }

        private Dictionary<BankType, Bank> m_banks;
        
        public Section_DATA(SectionHeader inHeader, Dictionary<BankType, Bank> banks)
            : base(inHeader)
        {
            m_banks = banks;
        }

        public override void Read(NativeReader reader)
        {
            Size = (int)reader.ReadUInt(m_endian);
            
            long bankStartPosition = reader.Position;

            reader.ReadULong(); // unknown
            reader.ReadULong(); // unknown
            BankType type = (BankType)reader.ReadULong();
            reader.ReadUInt(); // unknown
            ushort position = reader.ReadUShort();
            reader.ReadUShort(); // unknown

            Bank bank = m_banks[type];
            
            Type assetBankType = AssetBankTypeLibrary.GetType(bank.Name);
            object assetBankObject = Activator.CreateInstance(assetBankType);
        }
    }
}
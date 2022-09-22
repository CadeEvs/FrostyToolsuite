using System.Collections.Generic;
using System.Linq;
using FrostySdk.IO;

namespace AnimationEditorPlugin.Formats.Sections
{
    public class Pointer
    {
        private long m_offset;
        private long m_readerPosition;

        public Pointer(long inReaderPosition, long inOffset)
        {
            m_offset = inOffset;
            m_readerPosition = inReaderPosition;
        }

        public long GetPosition()
        {
            long offset = (int)(m_offset & 0x0fffffffffffffff);
            
            int flags = (int)(m_offset >> 60);
            if (flags == 1)
            {
                return m_readerPosition + offset;
            }
            
            return (flags == 0)
                ? offset
                : 0;
        }
    }
    
    public class Section_REF2 : Section_REFL
    {
        public Section_REF2(SectionHeader inHeader)
            : base(inHeader)
        {
        }

        public override void Read(NativeReader reader)
        {
            Size = m_headerSize - 12;
            
            //
            // banks
            //
            long bankStartPosition = reader.Position;
            
            ulong bankCount = reader.ReadULong(m_endian);
            Dictionary<long, Bank> banksPositions = new Dictionary<long, Bank>();
            for (ulong i = 0; i < bankCount; i++)
            {
                // this is always in little endian, not sure why
                Pointer pointer = new Pointer(reader.Position - bankStartPosition, reader.ReadLong(Endian.Little));
                
                banksPositions.Add(pointer.GetPosition(), new Bank(m_endian));
            }
            
            // parse banks
            Dictionary<BankType, Bank> banks = new Dictionary<BankType, Bank>();
            foreach (KeyValuePair<long, Bank> bank in banksPositions)
            {
                reader.Position = bankStartPosition + bank.Key;
                
                bank.Value.Read(reader, m_endian, bankStartPosition, banksPositions, 2);
                
                banks.Add(bank.Value.Type, bank.Value);
            }
            
            reader.BaseStream.Position = m_endPosition;
            
            Banks = banks;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using FrostySdk.IO;

namespace AnimationEditorPlugin.Formats.Sections
{
    public class Section_REFL : Section
    {
        public int Size { get; protected set; }
        public List<Bank> Banks { get; protected set; }
        
        public Section_REFL(SectionHeader inHeader)
            : base(inHeader)
        {
        }

        public override void Read(NativeReader reader)
        {
            Size = (int)reader.ReadUInt(m_endian);

            //
            // banks
            //
            long bankStartPosition = reader.Position;

            // setup banks
            ulong bankCount = reader.ReadULong(m_endian);
            Dictionary<long, Bank> banks = new Dictionary<long, Bank>();
            for (ulong i = 0; i < bankCount; i++)
            {
                long bankPosition = reader.ReadLong(m_endian);
                banks.Add(bankPosition, new Bank(m_endian));
            }
            
            // parse banks
            foreach (KeyValuePair<long, Bank> bank in banks)
            {
                reader.Position = bankStartPosition + bank.Key;
                
                bank.Value.Read(reader, m_endian, bankStartPosition, banks, 1);
            }
            
            reader.BaseStream.Position = m_endPosition;
            Banks = banks.Values.ToList();
        }
    }
}
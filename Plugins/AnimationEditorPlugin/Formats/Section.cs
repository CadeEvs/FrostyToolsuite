using System;
using System.Dynamic;
using AnimationEditorPlugin.Managers;
using FrostySdk.IO;

namespace AnimationEditorPlugin.Formats
{
    public enum SectionFormat
    {
        None,
        STRM,
        REFL,
        REF2,
        DATA,
        DAT2
    }

    public abstract class Section
    {
        protected Endian m_endian;
        
        public Section(SectionHeader inHeader)
        {
            m_endian = inHeader.Endian;
        }
        
        public abstract void Read(NativeReader reader);
    }
    
    public class SectionHeader
    {
        public SectionFormat Format { get; private set; }
        public Endian Endian { get; private set; }
        public int Size { get; private set; }

        public void Read(NativeReader reader)
        {
            // format
            string format = reader.ReadSizedString(7);
            switch (format)
            {
                case "GD.STRM":
                    Format = SectionFormat.STRM;
                    break;
                case "GD.REFL":
                    Format = SectionFormat.REFL;
                    break;
                case "GD.REF2":
                    Format = SectionFormat.REF2;
                    break;
                case "GD.DATA":
                    Format = SectionFormat.DATA;
                    break;
                case "GD.DAT2":
                    Format = SectionFormat.DAT2;
                    break;
            }

            // endian
            Endian = reader.ReadSizedString(1) == "b" 
                ? Endian.Big
                : Endian.Little;

            // size
            Size = reader.ReadInt(Endian);
        }
    }
    
    public class Section_STRM : Section
    {
        public uint LargestSectionSize { get; private set; }

        public Section_STRM(SectionHeader inHeader)
            : base(inHeader)
        {
        }
        
        public override void Read(NativeReader reader)
        {
            LargestSectionSize = reader.ReadUInt(m_endian);
        }
    }

    public class Section_REFL : Section
    {
        public Section_REFL(SectionHeader inHeader)
            : base(inHeader)
        {
        }

        public override void Read(NativeReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
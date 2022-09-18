using System;
using System.Collections.Generic;
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
        protected int m_headerSize;
        protected long m_endPosition;
        
        public Section(SectionHeader inHeader)
        {
            m_endian = inHeader.Endian;
            m_headerSize = inHeader.Size;
            m_endPosition = inHeader.ReaderPosition + (inHeader.Size - 12);
        }
        
        public abstract void Read(NativeReader reader);
    }
    
    public class SectionHeader
    {
        public SectionFormat Format { get; private set; }
        public Endian Endian { get; private set; }
        public int Size { get; private set; }

        // save this so section can get end position
        public long ReaderPosition { get; private set; }
        
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
            
            // reader position
            ReaderPosition = reader.BaseStream.Position;
        }
    }
}
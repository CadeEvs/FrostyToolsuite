using FrostySdk.IO;

namespace AnimationEditorPlugin.Formats.Sections
{
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
}
using FrostySdk.IO;

namespace AnimationEditorPlugin.Formats.Sections
{
    public class Section_DATA : Section
    {
        public Section_DATA(SectionHeader inHeader)
            : base(inHeader)
        {
        }

        public override void Read(NativeReader reader)
        {
            throw new System.NotImplementedException();
        }
    }
}
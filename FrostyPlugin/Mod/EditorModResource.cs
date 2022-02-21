using FrostySdk.IO;
using FrostySdk.Managers;

namespace Frosty.Core.Mod
{
    public class EditorModResource : BaseModResource
    {
        internal EditorModResource()
        {
        }

        public EditorModResource(AssetEntry entry)
            : base(entry)
        {
        }

        public virtual void Write(NativeWriter writer)
        {
            writer.Write((byte)Type);
            writer.Write(resourceIndex);
            writer.WriteNullTerminatedString(name);

            if (resourceIndex != -1)
            {
                writer.Write(sha1);
                writer.Write(size);
                writer.Write(flags);
                writer.Write(handlerHash);
                writer.WriteNullTerminatedString(userData);
            }

            writer.Write(bundlesToAdd.Count);
            foreach (int bname in bundlesToAdd)
                writer.Write(bname);
        }
    }
}

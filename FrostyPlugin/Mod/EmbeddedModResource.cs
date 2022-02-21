
namespace Frosty.Core.Mod
{
    public sealed class EmbeddedResource : BaseModResource
    {
        public EmbeddedResource()
        {
        }

        public override ModResourceType Type => ModResourceType.Embedded;
    }
}

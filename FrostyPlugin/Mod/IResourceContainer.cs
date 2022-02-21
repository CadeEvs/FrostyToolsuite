using System.Collections.Generic;

namespace Frosty.Core.Mod
{
    public interface IResourceContainer
    {
        IEnumerable<BaseModResource> Resources { get; }
        byte[] GetResourceData(BaseModResource resource);
    }
}

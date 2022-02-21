using System.Collections.Generic;
using System.IO;

namespace Frosty.Core.Mod
{
    public sealed class RuntimeResources : IResourceContainer
    {
        public IEnumerable<BaseModResource> Resources => resources;

        private List<BaseModResource> resources = new List<BaseModResource>();
        private List<byte[]> data = new List<byte[]>();

        public byte[] GetResourceData(BaseModResource resource)
        {
            int index = resources.IndexOf(resource);
            return data[index];
        }

        public void AddResource(BaseModResource resource, byte[] inData)
        {
            if (!(resource is RuntimeEbxResource) && !(resource is RuntimeResResource) && !(resource is RuntimeChunkResource))
                throw new InvalidDataException();

            resources.Add(resource);
            data.Add(inData);
        }
    }
}

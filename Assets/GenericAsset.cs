using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    public class GenericAsset<T> : Asset, IAssetData<T> where T : FrostySdk.Ebx.Asset
    {
        public T Data => data as T;

        public GenericAsset(Guid fileGuid, T inData)
            : base(fileGuid, inData)
        {
        }
    }
}

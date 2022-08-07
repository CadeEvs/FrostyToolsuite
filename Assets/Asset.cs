using FrostySdk.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    public class Asset : Entities.IEbxType
    {
        public Guid FileGuid { get; set; }
        public Guid InstanceGuid => data.__InstanceGuid.ExportedGuid;
        public string Name { get; protected set; }
        public uint NameHash { get; protected set; }

        protected FrostySdk.Ebx.Asset data;

        public Asset(Guid fileGuid, FrostySdk.Ebx.Asset inData)
        {
            FileGuid = fileGuid;
            data = inData;
            Name = data.Name;
            NameHash = (uint)Frosty.Hash.Fnv1.HashString(Name);
        }

        public FrostySdk.Ebx.PointerRef ToPointerRef()
        {
            return new FrostySdk.Ebx.PointerRef(new FrostySdk.IO.EbxImportReference() { FileGuid = FileGuid, ClassGuid = data.__InstanceGuid.ExportedGuid });
        }

        public virtual void Dispose()
        {
        }
    }

    public interface IAssetData<T>
    {
        T Data { get; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AssetBindingAttribute : Attribute
    {
        public Type DataType { get; set; }
    }
}

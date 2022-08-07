using FrostySdk.Attributes;
using FrostySdk.IO;
using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Entities
{
    public class WorldReferenceObjectData : FrostySdk.Ebx.SubWorldReferenceObjectData
    {
        [IsHidden]
        public FrostySdk.Ebx.WorldData WorldData { get; set; }
        [IsHidden]
        public EbxAsset EbxAsset { get; set; }
    }

    [EntityBinding(DataType = typeof(WorldReferenceObjectData))]
    public class WorldReferenceObject : SubWorldReferenceObject, IEntityData<WorldReferenceObjectData>
    {
        public new WorldReferenceObjectData Data => data as WorldReferenceObjectData;
        public new Assets.SubWorld Blueprint => blueprint as Assets.SubWorld;
        public Assets.SubWorld World { get; private set; }

        public WorldReferenceObject(Guid assetGuid, FrostySdk.Ebx.WorldData data, EbxAsset ebxAsset, EntityWorld inWorld)
            : base(new WorldReferenceObjectData() { Blueprint = new FrostySdk.Ebx.PointerRef(new FrostySdk.IO.EbxImportReference() { FileGuid = assetGuid, ClassGuid = data.GetInstanceGuid().ExportedGuid }), WorldData = data, AutoLoad = true, EbxAsset = ebxAsset }, null, inWorld)
        {
        }

        protected override void Initialize()
        {
            blueprint = LoadedAssetManager.Instance.LoadAsset<Assets.SubWorld>(Data.Blueprint.External.FileGuid, Data.WorldData, Data.EbxAsset);

            string networkRegistryName = $"{blueprint.Data.Name}_networkregistry_win32";
            string meshVariationName = $"{blueprint.Data.Name}/MeshVariationDb_Win32";

            NetworkRegistry = LoadedAssetManager.Instance.LoadAsset<Assets.NetworkRegistryAsset>(networkRegistryName);
            MeshVariationDatabase = LoadedAssetManager.Instance.LoadAsset<Assets.MeshVariationDatabase>(meshVariationName);
        }

        public override object GetPropertyGridData()
        {
            return Data.WorldData;
        }
    }
}

using LevelEditorPlugin.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Assets
{
    [AssetBinding(DataType = typeof(FrostySdk.Ebx.SchematicChannelAsset))]
    public class SchematicChannelAsset : Asset, IAssetData<FrostySdk.Ebx.SchematicChannelAsset>
    {
        public FrostySdk.Ebx.SchematicChannelAsset Data => data as FrostySdk.Ebx.SchematicChannelAsset;

        protected List<SchematicChannelEntity> entities = new List<SchematicChannelEntity>();

        public SchematicChannelAsset(Guid fileGuid, FrostySdk.Ebx.SchematicChannelAsset inData)
            : base(fileGuid, inData)
        {
        }

        public void AddEntity(SchematicChannelEntity inEntity)
        {
            entities.Add(inEntity);
        }

        public void OnEvent(SchematicChannelEntity callee, int eventHash)
        {
            foreach (var entity in entities)
            {
                if (entity == callee)
                    continue;

                var evt = entity.GetEvent(eventHash);
                if (evt != null)
                {
                    evt.Execute();
                }
            }
        }

        public void OnPropertyUpdated(SchematicChannelEntity callee, IProperty property)
        {
            foreach (var entity in entities)
            {
                if (entity == callee)
                    continue;

                var dstProperty = entity.GetProperty(property.NameHash);
                if (dstProperty != null)
                {
                    dstProperty.Value = property.Value;
                }
            }
        }
    }
}

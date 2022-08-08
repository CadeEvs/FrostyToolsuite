using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.GeometryTriggerEntityData))]
    public class GeometryTriggerEntity : AreaTriggerEntity, IEntityData<FrostySdk.Ebx.GeometryTriggerEntityData>
    {
        public new FrostySdk.Ebx.GeometryTriggerEntityData Data => data as FrostySdk.Ebx.GeometryTriggerEntityData;

        public GeometryTriggerEntity(FrostySdk.Ebx.GeometryTriggerEntityData inData, Entity inParent)
            : base(inData, inParent)
        {
        }
    }
}

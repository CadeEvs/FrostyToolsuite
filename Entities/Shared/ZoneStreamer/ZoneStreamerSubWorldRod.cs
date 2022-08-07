using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditorPlugin.Entities
{
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.ZoneStreamerSubWorldRod))]
    public class ZoneStreamerSubWorldRod : SubWorldReferenceObject, IEntityData<FrostySdk.Ebx.ZoneStreamerSubWorldRod>
    {
        public new FrostySdk.Ebx.ZoneStreamerSubWorldRod Data => data as FrostySdk.Ebx.ZoneStreamerSubWorldRod;

        public ZoneStreamerSubWorldRod(FrostySdk.Ebx.ZoneStreamerSubWorldRod inData, Entity inParent)
            : base(inData, inParent)
        {
        }
    }
}

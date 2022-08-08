using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ZoneStreamerEntityData))]
	public class ZoneStreamerEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.ZoneStreamerEntityData>
	{
		public new FrostySdk.Ebx.ZoneStreamerEntityData Data => data as FrostySdk.Ebx.ZoneStreamerEntityData;

		public ZoneStreamerEntity(FrostySdk.Ebx.ZoneStreamerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


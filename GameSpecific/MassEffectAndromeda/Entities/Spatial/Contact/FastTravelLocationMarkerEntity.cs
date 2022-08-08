using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FastTravelLocationMarkerEntityData))]
	public class FastTravelLocationMarkerEntity : LocationMarkerEntity, IEntityData<FrostySdk.Ebx.FastTravelLocationMarkerEntityData>
	{
		public new FrostySdk.Ebx.FastTravelLocationMarkerEntityData Data => data as FrostySdk.Ebx.FastTravelLocationMarkerEntityData;

		public FastTravelLocationMarkerEntity(FrostySdk.Ebx.FastTravelLocationMarkerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


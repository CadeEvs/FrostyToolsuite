using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InterFloorMarkerEntityData))]
	public class InterFloorMarkerEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.InterFloorMarkerEntityData>
	{
		public new FrostySdk.Ebx.InterFloorMarkerEntityData Data => data as FrostySdk.Ebx.InterFloorMarkerEntityData;

		public InterFloorMarkerEntity(FrostySdk.Ebx.InterFloorMarkerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


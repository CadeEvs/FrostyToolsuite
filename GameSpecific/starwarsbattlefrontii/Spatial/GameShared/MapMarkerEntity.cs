using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MapMarkerEntityData))]
	public class MapMarkerEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.MapMarkerEntityData>
	{
		public new FrostySdk.Ebx.MapMarkerEntityData Data => data as FrostySdk.Ebx.MapMarkerEntityData;

		public MapMarkerEntity(FrostySdk.Ebx.MapMarkerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


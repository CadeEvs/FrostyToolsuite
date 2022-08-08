using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MapWaypointEntityData))]
	public class MapWaypointEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.MapWaypointEntityData>
	{
		public new FrostySdk.Ebx.MapWaypointEntityData Data => data as FrostySdk.Ebx.MapWaypointEntityData;

		public MapWaypointEntity(FrostySdk.Ebx.MapWaypointEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


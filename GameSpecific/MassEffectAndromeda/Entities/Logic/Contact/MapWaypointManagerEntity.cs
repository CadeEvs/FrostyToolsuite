using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MapWaypointManagerEntityData))]
	public class MapWaypointManagerEntity : SingletonEntity, IEntityData<FrostySdk.Ebx.MapWaypointManagerEntityData>
	{
		public new FrostySdk.Ebx.MapWaypointManagerEntityData Data => data as FrostySdk.Ebx.MapWaypointManagerEntityData;
		public override string DisplayName => "MapWaypointManager";

		public MapWaypointManagerEntity(FrostySdk.Ebx.MapWaypointManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


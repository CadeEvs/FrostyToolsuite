using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetWaypointsEntityData))]
	public class SetWaypointsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetWaypointsEntityData>
	{
		public new FrostySdk.Ebx.SetWaypointsEntityData Data => data as FrostySdk.Ebx.SetWaypointsEntityData;
		public override string DisplayName => "SetWaypoints";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SetWaypointsEntity(FrostySdk.Ebx.SetWaypointsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaypointTriggerEntityData))]
	public class WaypointTriggerEntity : WaypointTriggerEntityBase, IEntityData<FrostySdk.Ebx.WaypointTriggerEntityData>
	{
		public new FrostySdk.Ebx.WaypointTriggerEntityData Data => data as FrostySdk.Ebx.WaypointTriggerEntityData;
		public override string DisplayName => "WaypointTrigger";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WaypointTriggerEntity(FrostySdk.Ebx.WaypointTriggerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


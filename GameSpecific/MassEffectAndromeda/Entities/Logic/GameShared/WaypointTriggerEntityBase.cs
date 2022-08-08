using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WaypointTriggerEntityBaseData))]
	public class WaypointTriggerEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.WaypointTriggerEntityBaseData>
	{
		public new FrostySdk.Ebx.WaypointTriggerEntityBaseData Data => data as FrostySdk.Ebx.WaypointTriggerEntityBaseData;
		public override string DisplayName => "WaypointTriggerEntityBase";

		public WaypointTriggerEntityBase(FrostySdk.Ebx.WaypointTriggerEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


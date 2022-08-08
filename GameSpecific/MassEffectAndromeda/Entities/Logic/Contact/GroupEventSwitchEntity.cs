using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GroupEventSwitchEntityData))]
	public class GroupEventSwitchEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GroupEventSwitchEntityData>
	{
		public new FrostySdk.Ebx.GroupEventSwitchEntityData Data => data as FrostySdk.Ebx.GroupEventSwitchEntityData;
		public override string DisplayName => "GroupEventSwitch";

		public GroupEventSwitchEntity(FrostySdk.Ebx.GroupEventSwitchEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


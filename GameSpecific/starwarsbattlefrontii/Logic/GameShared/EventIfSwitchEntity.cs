using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventIfSwitchEntityData))]
	public class EventIfSwitchEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EventIfSwitchEntityData>
	{
		public new FrostySdk.Ebx.EventIfSwitchEntityData Data => data as FrostySdk.Ebx.EventIfSwitchEntityData;
		public override string DisplayName => "EventIfSwitch";

		public EventIfSwitchEntity(FrostySdk.Ebx.EventIfSwitchEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


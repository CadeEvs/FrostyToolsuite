using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventAndGateExEntityData))]
	public class EventAndGateExEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EventAndGateExEntityData>
	{
		public new FrostySdk.Ebx.EventAndGateExEntityData Data => data as FrostySdk.Ebx.EventAndGateExEntityData;
		public override string DisplayName => "EventAndGateEx";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EventAndGateExEntity(FrostySdk.Ebx.EventAndGateExEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


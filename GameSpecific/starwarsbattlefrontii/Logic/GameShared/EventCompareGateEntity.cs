using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventCompareGateEntityData))]
	public class EventCompareGateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EventCompareGateEntityData>
	{
		public new FrostySdk.Ebx.EventCompareGateEntityData Data => data as FrostySdk.Ebx.EventCompareGateEntityData;
		public override string DisplayName => "EventCompareGate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EventCompareGateEntity(FrostySdk.Ebx.EventCompareGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


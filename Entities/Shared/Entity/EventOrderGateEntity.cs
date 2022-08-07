using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EventOrderGateEntityData))]
	public class EventOrderGateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EventOrderGateEntityData>
	{
		public new FrostySdk.Ebx.EventOrderGateEntityData Data => data as FrostySdk.Ebx.EventOrderGateEntityData;
		public override string DisplayName => "EventOrderGate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EventOrderGateEntity(FrostySdk.Ebx.EventOrderGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


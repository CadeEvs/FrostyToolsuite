using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StateEventGateEntityData))]
	public class StateEventGateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StateEventGateEntityData>
	{
		public new FrostySdk.Ebx.StateEventGateEntityData Data => data as FrostySdk.Ebx.StateEventGateEntityData;
		public override string DisplayName => "StateEventGate";

		public StateEventGateEntity(FrostySdk.Ebx.StateEventGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


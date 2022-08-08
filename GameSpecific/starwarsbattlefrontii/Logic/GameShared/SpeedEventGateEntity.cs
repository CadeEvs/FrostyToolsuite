using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpeedEventGateEntityData))]
	public class SpeedEventGateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SpeedEventGateEntityData>
	{
		public new FrostySdk.Ebx.SpeedEventGateEntityData Data => data as FrostySdk.Ebx.SpeedEventGateEntityData;
		public override string DisplayName => "SpeedEventGate";

		public SpeedEventGateEntity(FrostySdk.Ebx.SpeedEventGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


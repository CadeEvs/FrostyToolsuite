using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LogitechLEDPulseEffectEntityData))]
	public class LogitechLEDPulseEffectEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LogitechLEDPulseEffectEntityData>
	{
		public new FrostySdk.Ebx.LogitechLEDPulseEffectEntityData Data => data as FrostySdk.Ebx.LogitechLEDPulseEffectEntityData;
		public override string DisplayName => "LogitechLEDPulseEffect";

		public LogitechLEDPulseEffectEntity(FrostySdk.Ebx.LogitechLEDPulseEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


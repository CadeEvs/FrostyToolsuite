using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LogitechLEDConstantEffectEntityData))]
	public class LogitechLEDConstantEffectEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LogitechLEDConstantEffectEntityData>
	{
		public new FrostySdk.Ebx.LogitechLEDConstantEffectEntityData Data => data as FrostySdk.Ebx.LogitechLEDConstantEffectEntityData;
		public override string DisplayName => "LogitechLEDConstantEffect";

		public LogitechLEDConstantEffectEntity(FrostySdk.Ebx.LogitechLEDConstantEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


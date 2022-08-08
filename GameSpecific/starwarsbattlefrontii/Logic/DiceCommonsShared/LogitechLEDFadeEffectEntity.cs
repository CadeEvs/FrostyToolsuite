using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LogitechLEDFadeEffectEntityData))]
	public class LogitechLEDFadeEffectEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LogitechLEDFadeEffectEntityData>
	{
		public new FrostySdk.Ebx.LogitechLEDFadeEffectEntityData Data => data as FrostySdk.Ebx.LogitechLEDFadeEffectEntityData;
		public override string DisplayName => "LogitechLEDFadeEffect";

		public LogitechLEDFadeEffectEntity(FrostySdk.Ebx.LogitechLEDFadeEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


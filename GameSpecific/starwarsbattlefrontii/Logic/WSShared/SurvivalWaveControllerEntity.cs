using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SurvivalWaveControllerEntityData))]
	public class SurvivalWaveControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SurvivalWaveControllerEntityData>
	{
		public new FrostySdk.Ebx.SurvivalWaveControllerEntityData Data => data as FrostySdk.Ebx.SurvivalWaveControllerEntityData;
		public override string DisplayName => "SurvivalWaveController";

		public SurvivalWaveControllerEntity(FrostySdk.Ebx.SurvivalWaveControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


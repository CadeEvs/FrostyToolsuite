using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GloballFallbackStageCameraEntityData))]
	public class GloballFallbackStageCameraEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GloballFallbackStageCameraEntityData>
	{
		public new FrostySdk.Ebx.GloballFallbackStageCameraEntityData Data => data as FrostySdk.Ebx.GloballFallbackStageCameraEntityData;
		public override string DisplayName => "GloballFallbackStageCamera";

		public GloballFallbackStageCameraEntity(FrostySdk.Ebx.GloballFallbackStageCameraEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


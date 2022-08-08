using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioCurveFactorEntityData))]
	public class AudioCurveFactorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AudioCurveFactorEntityData>
	{
		public new FrostySdk.Ebx.AudioCurveFactorEntityData Data => data as FrostySdk.Ebx.AudioCurveFactorEntityData;
		public override string DisplayName => "AudioCurveFactor";

		public AudioCurveFactorEntity(FrostySdk.Ebx.AudioCurveFactorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


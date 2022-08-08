using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioRaycastCalculationEntityData))]
	public class AudioRaycastCalculationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AudioRaycastCalculationEntityData>
	{
		public new FrostySdk.Ebx.AudioRaycastCalculationEntityData Data => data as FrostySdk.Ebx.AudioRaycastCalculationEntityData;
		public override string DisplayName => "AudioRaycastCalculation";

		public AudioRaycastCalculationEntity(FrostySdk.Ebx.AudioRaycastCalculationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


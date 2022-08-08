using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioAssetValidationEntityData))]
	public class AudioAssetValidationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AudioAssetValidationEntityData>
	{
		public new FrostySdk.Ebx.AudioAssetValidationEntityData Data => data as FrostySdk.Ebx.AudioAssetValidationEntityData;
		public override string DisplayName => "AudioAssetValidation";

		public AudioAssetValidationEntity(FrostySdk.Ebx.AudioAssetValidationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


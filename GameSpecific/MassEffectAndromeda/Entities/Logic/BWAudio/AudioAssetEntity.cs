using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioAssetEntityData))]
	public class AudioAssetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AudioAssetEntityData>
	{
		public new FrostySdk.Ebx.AudioAssetEntityData Data => data as FrostySdk.Ebx.AudioAssetEntityData;
		public override string DisplayName => "AudioAsset";

		public AudioAssetEntity(FrostySdk.Ebx.AudioAssetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


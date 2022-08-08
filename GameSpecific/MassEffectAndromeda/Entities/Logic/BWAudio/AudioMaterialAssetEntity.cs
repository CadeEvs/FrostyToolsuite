using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AudioMaterialAssetEntityData))]
	public class AudioMaterialAssetEntity : AudioAssetEntity, IEntityData<FrostySdk.Ebx.AudioMaterialAssetEntityData>
	{
		public new FrostySdk.Ebx.AudioMaterialAssetEntityData Data => data as FrostySdk.Ebx.AudioMaterialAssetEntityData;
		public override string DisplayName => "AudioMaterialAsset";

		public AudioMaterialAssetEntity(FrostySdk.Ebx.AudioMaterialAssetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


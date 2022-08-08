using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoundAssetDataEntityData))]
	public class SoundAssetDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SoundAssetDataEntityData>
	{
		public new FrostySdk.Ebx.SoundAssetDataEntityData Data => data as FrostySdk.Ebx.SoundAssetDataEntityData;
		public override string DisplayName => "SoundAssetData";

		public SoundAssetDataEntity(FrostySdk.Ebx.SoundAssetDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


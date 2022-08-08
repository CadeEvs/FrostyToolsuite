using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TextureAssetEntityData))]
	public class TextureAssetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TextureAssetEntityData>
	{
		public new FrostySdk.Ebx.TextureAssetEntityData Data => data as FrostySdk.Ebx.TextureAssetEntityData;
		public override string DisplayName => "TextureAsset";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TextureAssetEntity(FrostySdk.Ebx.TextureAssetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


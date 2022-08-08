using LevelEditorPlugin.Managers;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TextureAssetEntityData))]
	public class TextureAssetEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TextureAssetEntityData>
	{
		public new FrostySdk.Ebx.TextureAssetEntityData Data => data as FrostySdk.Ebx.TextureAssetEntityData;
		public override string DisplayName => "TextureAsset";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		protected Property<Assets.TextureAsset> textureProperty;
		protected Assets.TextureAsset textureAsset;

		public TextureAssetEntity(FrostySdk.Ebx.TextureAssetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			textureProperty = new Property<Assets.TextureAsset>(this, 0x6bde20ba);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();

			if (textureProperty.IsUnset)
			{
				textureAsset = LoadedAssetManager.Instance.LoadAsset<Assets.TextureAsset>(this, Data.DefaultTextureAsset);
				textureProperty.Value = textureAsset;
			}
		}

        public override void EndSimulation()
        {
            base.EndSimulation();
			LoadedAssetManager.Instance.UnloadAsset(textureAsset);
        }
    }
}


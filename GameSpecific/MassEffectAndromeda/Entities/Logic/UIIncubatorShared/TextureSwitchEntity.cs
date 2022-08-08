using LevelEditorPlugin.Managers;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TextureSwitchEntityData))]
	public class TextureSwitchEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TextureSwitchEntityData>
	{
		protected readonly int Property_Index = Frosty.Hash.Fnv1.HashString("Index");
		protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.TextureSwitchEntityData Data => data as FrostySdk.Ebx.TextureSwitchEntityData;
		public override string DisplayName => "TextureSwitch";
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Index", Direction.In, typeof(int)),
				new ConnectionDesc("Out", Direction.Out, typeof(Assets.TextureAsset))
			};
        }

		protected Property<int> indexProperty;
		protected Property<Assets.TextureAsset> outProperty;
		protected List<Assets.TextureAsset> textures = new List<Assets.TextureAsset>();

        public TextureSwitchEntity(FrostySdk.Ebx.TextureSwitchEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			indexProperty = new Property<int>(this, Property_Index, 0);
			outProperty = new Property<Assets.TextureAsset>(this, Property_Out);

			for (int i = 0; i < Data.Textures.Count; i++)
            {
				textures.Add(null);
            }				
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == indexProperty.NameHash)
			{
				int currentIndex = indexProperty.Value;
				if (textures[currentIndex] == null)
				{
					textures[currentIndex] = LoadedAssetManager.Instance.LoadAsset<Assets.TextureAsset>(this, Data.Textures[currentIndex]);
				}

				outProperty.Value = textures[currentIndex];
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }

        public override void Destroy()
        {
			foreach (var texture in textures)
			{
				LoadedAssetManager.Instance.UnloadAsset(texture);
			}

            base.Destroy();
        }
    }
}


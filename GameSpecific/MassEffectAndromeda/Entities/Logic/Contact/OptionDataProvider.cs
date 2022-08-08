using LevelEditorPlugin.Managers;
using System.Collections.Generic;
using System.IO;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OptionDataProviderData))]
	public class OptionDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.OptionDataProviderData>
	{
		public new FrostySdk.Ebx.OptionDataProviderData Data => data as FrostySdk.Ebx.OptionDataProviderData;
		public override string DisplayName => "OptionDataProvider";
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Set", Direction.In),
				new ConnectionDesc("Update", Direction.In),
				new ConnectionDesc("Apply", Direction.In)
			};
		}
        public override IEnumerable<ConnectionDesc> Properties
        {
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				if (optionAsset != null)
				{
					string optionType = optionAsset.OptionType;
					if (!string.IsNullOrEmpty(optionType))
                    {
						outProperties.Add(new ConnectionDesc() { Name = $"{optionType}Value", Direction = Direction.Out });
                    }
				}
				return outProperties;
			}
		}
		public override IEnumerable<string> HeaderRows
		{
			get
			{
				List<string> outHeaderRows = new List<string>();
				if (optionAsset != null)
				{
					outHeaderRows.Add(Path.GetFileName(optionAsset.Name));
				}
				return outHeaderRows;
			}
		}

		private Assets.ProfileOptionData optionAsset;

        public OptionDataProvider(FrostySdk.Ebx.OptionDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
			optionAsset = LoadedAssetManager.Instance.LoadAsset<Assets.ProfileOptionData>(this, Data.Option);
		}

        public override void Destroy()
        {
			LoadedAssetManager.Instance.UnloadAsset(optionAsset);
        }
    }
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ColorPresetDataProviderData))]
	public class ColorPresetDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.ColorPresetDataProviderData>
	{
		public new FrostySdk.Ebx.ColorPresetDataProviderData Data => data as FrostySdk.Ebx.ColorPresetDataProviderData;
		public override string DisplayName => "ColorPresetDataProvider";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ColorPresetDataProvider(FrostySdk.Ebx.ColorPresetDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


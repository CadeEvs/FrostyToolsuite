using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SliderSelectorDataProviderData))]
	public class SliderSelectorDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.SliderSelectorDataProviderData>
	{
		public new FrostySdk.Ebx.SliderSelectorDataProviderData Data => data as FrostySdk.Ebx.SliderSelectorDataProviderData;
		public override string DisplayName => "SliderSelectorDataProvider";

		public SliderSelectorDataProvider(FrostySdk.Ebx.SliderSelectorDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


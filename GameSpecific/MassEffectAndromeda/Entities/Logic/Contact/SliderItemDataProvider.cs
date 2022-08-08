using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SliderItemDataProviderData))]
	public class SliderItemDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.SliderItemDataProviderData>
	{
		public new FrostySdk.Ebx.SliderItemDataProviderData Data => data as FrostySdk.Ebx.SliderItemDataProviderData;
		public override string DisplayName => "SliderItemDataProvider";

		public SliderItemDataProvider(FrostySdk.Ebx.SliderItemDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


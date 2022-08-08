using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OptionsDataProviderData))]
	public class OptionsDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.OptionsDataProviderData>
	{
		public new FrostySdk.Ebx.OptionsDataProviderData Data => data as FrostySdk.Ebx.OptionsDataProviderData;
		public override string DisplayName => "OptionsDataProvider";

		public OptionsDataProvider(FrostySdk.Ebx.OptionsDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


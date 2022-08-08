using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PerksUIDataProviderData))]
	public class PerksUIDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.PerksUIDataProviderData>
	{
		public new FrostySdk.Ebx.PerksUIDataProviderData Data => data as FrostySdk.Ebx.PerksUIDataProviderData;
		public override string DisplayName => "PerksUIDataProvider";

		public PerksUIDataProvider(FrostySdk.Ebx.PerksUIDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StatsUIDataProviderData))]
	public class StatsUIDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.StatsUIDataProviderData>
	{
		public new FrostySdk.Ebx.StatsUIDataProviderData Data => data as FrostySdk.Ebx.StatsUIDataProviderData;
		public override string DisplayName => "StatsUIDataProvider";

		public StatsUIDataProvider(FrostySdk.Ebx.StatsUIDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


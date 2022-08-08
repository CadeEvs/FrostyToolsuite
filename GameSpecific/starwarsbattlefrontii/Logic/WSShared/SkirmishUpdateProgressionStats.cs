using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SkirmishUpdateProgressionStatsData))]
	public class SkirmishUpdateProgressionStats : LogicEntity, IEntityData<FrostySdk.Ebx.SkirmishUpdateProgressionStatsData>
	{
		public new FrostySdk.Ebx.SkirmishUpdateProgressionStatsData Data => data as FrostySdk.Ebx.SkirmishUpdateProgressionStatsData;
		public override string DisplayName => "SkirmishUpdateProgressionStats";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SkirmishUpdateProgressionStats(FrostySdk.Ebx.SkirmishUpdateProgressionStatsData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


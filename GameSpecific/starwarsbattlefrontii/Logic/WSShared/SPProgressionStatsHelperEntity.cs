using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPProgressionStatsHelperEntityData))]
	public class SPProgressionStatsHelperEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPProgressionStatsHelperEntityData>
	{
		public new FrostySdk.Ebx.SPProgressionStatsHelperEntityData Data => data as FrostySdk.Ebx.SPProgressionStatsHelperEntityData;
		public override string DisplayName => "SPProgressionStatsHelper";

		public SPProgressionStatsHelperEntity(FrostySdk.Ebx.SPProgressionStatsHelperEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


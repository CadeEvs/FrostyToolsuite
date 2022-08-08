using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIStatsEntityData))]
	public class UIStatsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIStatsEntityData>
	{
		public new FrostySdk.Ebx.UIStatsEntityData Data => data as FrostySdk.Ebx.UIStatsEntityData;
		public override string DisplayName => "UIStats";

		public UIStatsEntity(FrostySdk.Ebx.UIStatsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


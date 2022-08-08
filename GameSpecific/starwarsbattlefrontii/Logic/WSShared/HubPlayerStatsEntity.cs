using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HubPlayerStatsEntityData))]
	public class HubPlayerStatsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HubPlayerStatsEntityData>
	{
		public new FrostySdk.Ebx.HubPlayerStatsEntityData Data => data as FrostySdk.Ebx.HubPlayerStatsEntityData;
		public override string DisplayName => "HubPlayerStats";

		public HubPlayerStatsEntity(FrostySdk.Ebx.HubPlayerStatsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


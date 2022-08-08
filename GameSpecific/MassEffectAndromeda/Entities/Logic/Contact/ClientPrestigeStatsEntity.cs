using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientPrestigeStatsEntityData))]
	public class ClientPrestigeStatsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientPrestigeStatsEntityData>
	{
		public new FrostySdk.Ebx.ClientPrestigeStatsEntityData Data => data as FrostySdk.Ebx.ClientPrestigeStatsEntityData;
		public override string DisplayName => "ClientPrestigeStats";

		public ClientPrestigeStatsEntity(FrostySdk.Ebx.ClientPrestigeStatsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerKillCounterEntityData))]
	public class PlayerKillCounterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerKillCounterEntityData>
	{
		public new FrostySdk.Ebx.PlayerKillCounterEntityData Data => data as FrostySdk.Ebx.PlayerKillCounterEntityData;
		public override string DisplayName => "PlayerKillCounter";

		public PlayerKillCounterEntity(FrostySdk.Ebx.PlayerKillCounterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


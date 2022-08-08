using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerEventCacheEntityData))]
	public class PlayerEventCacheEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerEventCacheEntityData>
	{
		public new FrostySdk.Ebx.PlayerEventCacheEntityData Data => data as FrostySdk.Ebx.PlayerEventCacheEntityData;
		public override string DisplayName => "PlayerEventCache";

		public PlayerEventCacheEntity(FrostySdk.Ebx.PlayerEventCacheEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


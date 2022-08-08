using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerEventEntityData))]
	public class PlayerEventEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerEventEntityData>
	{
		public new FrostySdk.Ebx.PlayerEventEntityData Data => data as FrostySdk.Ebx.PlayerEventEntityData;
		public override string DisplayName => "PlayerEvent";

		public PlayerEventEntity(FrostySdk.Ebx.PlayerEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


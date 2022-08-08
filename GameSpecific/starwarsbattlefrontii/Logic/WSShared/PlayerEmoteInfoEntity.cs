using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerEmoteInfoEntityData))]
	public class PlayerEmoteInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerEmoteInfoEntityData>
	{
		public new FrostySdk.Ebx.PlayerEmoteInfoEntityData Data => data as FrostySdk.Ebx.PlayerEmoteInfoEntityData;
		public override string DisplayName => "PlayerEmoteInfo";

		public PlayerEmoteInfoEntity(FrostySdk.Ebx.PlayerEmoteInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


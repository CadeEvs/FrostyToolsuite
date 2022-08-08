using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerLoadoutInfoEntityData))]
	public class PlayerLoadoutInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerLoadoutInfoEntityData>
	{
		public new FrostySdk.Ebx.PlayerLoadoutInfoEntityData Data => data as FrostySdk.Ebx.PlayerLoadoutInfoEntityData;
		public override string DisplayName => "PlayerLoadoutInfo";

		public PlayerLoadoutInfoEntity(FrostySdk.Ebx.PlayerLoadoutInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


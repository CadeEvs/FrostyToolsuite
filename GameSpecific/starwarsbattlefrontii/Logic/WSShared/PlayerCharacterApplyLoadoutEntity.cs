using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerCharacterApplyLoadoutEntityData))]
	public class PlayerCharacterApplyLoadoutEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerCharacterApplyLoadoutEntityData>
	{
		public new FrostySdk.Ebx.PlayerCharacterApplyLoadoutEntityData Data => data as FrostySdk.Ebx.PlayerCharacterApplyLoadoutEntityData;
		public override string DisplayName => "PlayerCharacterApplyLoadout";

		public PlayerCharacterApplyLoadoutEntity(FrostySdk.Ebx.PlayerCharacterApplyLoadoutEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerAbilityInfoEntityData))]
	public class PlayerAbilityInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerAbilityInfoEntityData>
	{
		public new FrostySdk.Ebx.PlayerAbilityInfoEntityData Data => data as FrostySdk.Ebx.PlayerAbilityInfoEntityData;
		public override string DisplayName => "PlayerAbilityInfo";

		public PlayerAbilityInfoEntity(FrostySdk.Ebx.PlayerAbilityInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


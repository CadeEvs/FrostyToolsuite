using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerAbilityStateEntityData))]
	public class PlayerAbilityStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerAbilityStateEntityData>
	{
		public new FrostySdk.Ebx.PlayerAbilityStateEntityData Data => data as FrostySdk.Ebx.PlayerAbilityStateEntityData;
		public override string DisplayName => "PlayerAbilityState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlayerAbilityStateEntity(FrostySdk.Ebx.PlayerAbilityStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


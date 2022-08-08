using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerAbilityPropertyOutputEntityData))]
	public class PlayerAbilityPropertyOutputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerAbilityPropertyOutputEntityData>
	{
		public new FrostySdk.Ebx.PlayerAbilityPropertyOutputEntityData Data => data as FrostySdk.Ebx.PlayerAbilityPropertyOutputEntityData;
		public override string DisplayName => "PlayerAbilityPropertyOutput";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlayerAbilityPropertyOutputEntity(FrostySdk.Ebx.PlayerAbilityPropertyOutputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


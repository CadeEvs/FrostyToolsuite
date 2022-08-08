using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EmoteCharacterStatePlayerAbilityEntityData))]
	public class EmoteCharacterStatePlayerAbilityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.EmoteCharacterStatePlayerAbilityEntityData>
	{
		public new FrostySdk.Ebx.EmoteCharacterStatePlayerAbilityEntityData Data => data as FrostySdk.Ebx.EmoteCharacterStatePlayerAbilityEntityData;
		public override string DisplayName => "EmoteCharacterStatePlayerAbility";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public EmoteCharacterStatePlayerAbilityEntity(FrostySdk.Ebx.EmoteCharacterStatePlayerAbilityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


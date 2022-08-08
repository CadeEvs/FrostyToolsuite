using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CategoryToPlayerAbilityEntityData))]
	public class CategoryToPlayerAbilityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CategoryToPlayerAbilityEntityData>
	{
		public new FrostySdk.Ebx.CategoryToPlayerAbilityEntityData Data => data as FrostySdk.Ebx.CategoryToPlayerAbilityEntityData;
		public override string DisplayName => "CategoryToPlayerAbility";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CategoryToPlayerAbilityEntity(FrostySdk.Ebx.CategoryToPlayerAbilityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


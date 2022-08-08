using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnWeaponAttackEntityData))]
	public class OnWeaponAttackEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnWeaponAttackEntityData>
	{
		public new FrostySdk.Ebx.OnWeaponAttackEntityData Data => data as FrostySdk.Ebx.OnWeaponAttackEntityData;
		public override string DisplayName => "OnWeaponAttack";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public OnWeaponAttackEntity(FrostySdk.Ebx.OnWeaponAttackEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeaponModifierStateEntityData))]
	public class WeaponModifierStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WeaponModifierStateEntityData>
	{
		public new FrostySdk.Ebx.WeaponModifierStateEntityData Data => data as FrostySdk.Ebx.WeaponModifierStateEntityData;
		public override string DisplayName => "WeaponModifierState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WeaponModifierStateEntity(FrostySdk.Ebx.WeaponModifierStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


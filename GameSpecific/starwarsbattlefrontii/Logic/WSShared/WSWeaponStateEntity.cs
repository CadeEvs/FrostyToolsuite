using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSWeaponStateEntityData))]
	public class WSWeaponStateEntity : WeaponStateEntity, IEntityData<FrostySdk.Ebx.WSWeaponStateEntityData>
	{
		public new FrostySdk.Ebx.WSWeaponStateEntityData Data => data as FrostySdk.Ebx.WSWeaponStateEntityData;
		public override string DisplayName => "WSWeaponState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSWeaponStateEntity(FrostySdk.Ebx.WSWeaponStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


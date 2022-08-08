using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeaponStateEntityData))]
	public class WeaponStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WeaponStateEntityData>
	{
		public new FrostySdk.Ebx.WeaponStateEntityData Data => data as FrostySdk.Ebx.WeaponStateEntityData;
		public override string DisplayName => "WeaponState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WeaponStateEntity(FrostySdk.Ebx.WeaponStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


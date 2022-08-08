using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSAlternatingShotWeaponStateEntityData))]
	public class WSAlternatingShotWeaponStateEntity : WSWeaponStateEntity, IEntityData<FrostySdk.Ebx.WSAlternatingShotWeaponStateEntityData>
	{
		public new FrostySdk.Ebx.WSAlternatingShotWeaponStateEntityData Data => data as FrostySdk.Ebx.WSAlternatingShotWeaponStateEntityData;
		public override string DisplayName => "WSAlternatingShotWeaponState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSAlternatingShotWeaponStateEntity(FrostySdk.Ebx.WSAlternatingShotWeaponStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


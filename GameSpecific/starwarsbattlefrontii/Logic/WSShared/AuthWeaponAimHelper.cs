using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AuthWeaponAimHelperData))]
	public class AuthWeaponAimHelper : LogicEntity, IEntityData<FrostySdk.Ebx.AuthWeaponAimHelperData>
	{
		public new FrostySdk.Ebx.AuthWeaponAimHelperData Data => data as FrostySdk.Ebx.AuthWeaponAimHelperData;
		public override string DisplayName => "AuthWeaponAimHelper";

		public AuthWeaponAimHelper(FrostySdk.Ebx.AuthWeaponAimHelperData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeaponEnablerEntityData))]
	public class WeaponEnablerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WeaponEnablerEntityData>
	{
		public new FrostySdk.Ebx.WeaponEnablerEntityData Data => data as FrostySdk.Ebx.WeaponEnablerEntityData;
		public override string DisplayName => "WeaponEnabler";

		public WeaponEnablerEntity(FrostySdk.Ebx.WeaponEnablerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


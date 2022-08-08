using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WeaponLagEntityData))]
	public class WeaponLagEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WeaponLagEntityData>
	{
		public new FrostySdk.Ebx.WeaponLagEntityData Data => data as FrostySdk.Ebx.WeaponLagEntityData;
		public override string DisplayName => "WeaponLag";

		public WeaponLagEntity(FrostySdk.Ebx.WeaponLagEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


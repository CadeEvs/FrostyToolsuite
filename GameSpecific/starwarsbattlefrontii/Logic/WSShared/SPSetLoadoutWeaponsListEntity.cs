using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPSetLoadoutWeaponsListEntityData))]
	public class SPSetLoadoutWeaponsListEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPSetLoadoutWeaponsListEntityData>
	{
		public new FrostySdk.Ebx.SPSetLoadoutWeaponsListEntityData Data => data as FrostySdk.Ebx.SPSetLoadoutWeaponsListEntityData;
		public override string DisplayName => "SPSetLoadoutWeaponsList";

		public SPSetLoadoutWeaponsListEntity(FrostySdk.Ebx.SPSetLoadoutWeaponsListEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


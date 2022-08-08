using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SPGetLoadoutWeaponsListEntityData))]
	public class SPGetLoadoutWeaponsListEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SPGetLoadoutWeaponsListEntityData>
	{
		public new FrostySdk.Ebx.SPGetLoadoutWeaponsListEntityData Data => data as FrostySdk.Ebx.SPGetLoadoutWeaponsListEntityData;
		public override string DisplayName => "SPGetLoadoutWeaponsList";

		public SPGetLoadoutWeaponsListEntity(FrostySdk.Ebx.SPGetLoadoutWeaponsListEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


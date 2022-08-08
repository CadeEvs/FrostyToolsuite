using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEEquipmentGetMaxAmmoData))]
	public class MEEquipmentGetMaxAmmo : LogicEntity, IEntityData<FrostySdk.Ebx.MEEquipmentGetMaxAmmoData>
	{
		public new FrostySdk.Ebx.MEEquipmentGetMaxAmmoData Data => data as FrostySdk.Ebx.MEEquipmentGetMaxAmmoData;
		public override string DisplayName => "MEEquipmentGetMaxAmmo";
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Soldier", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Get", Direction.In)
			};
		}

		public MEEquipmentGetMaxAmmo(FrostySdk.Ebx.MEEquipmentGetMaxAmmoData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


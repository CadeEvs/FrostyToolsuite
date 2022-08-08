using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MakoEquipmentManagerData))]
	public class MakoEquipmentManager : LogicEntity, IEntityData<FrostySdk.Ebx.MakoEquipmentManagerData>
	{
		public new FrostySdk.Ebx.MakoEquipmentManagerData Data => data as FrostySdk.Ebx.MakoEquipmentManagerData;
		public override string DisplayName => "MakoEquipmentManager";

		public MakoEquipmentManager(FrostySdk.Ebx.MakoEquipmentManagerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


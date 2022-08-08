using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ClientTokenEquipEntityData))]
	public class ClientTokenEquipEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ClientTokenEquipEntityData>
	{
		public new FrostySdk.Ebx.ClientTokenEquipEntityData Data => data as FrostySdk.Ebx.ClientTokenEquipEntityData;
		public override string DisplayName => "ClientTokenEquip";

		public ClientTokenEquipEntity(FrostySdk.Ebx.ClientTokenEquipEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


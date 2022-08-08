using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEItemStackablePickupEntityData))]
	public class MEItemStackablePickupEntity : MEItemActionEntity, IEntityData<FrostySdk.Ebx.MEItemStackablePickupEntityData>
	{
		public new FrostySdk.Ebx.MEItemStackablePickupEntityData Data => data as FrostySdk.Ebx.MEItemStackablePickupEntityData;
		public override string DisplayName => "MEItemStackablePickup";

		public MEItemStackablePickupEntity(FrostySdk.Ebx.MEItemStackablePickupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


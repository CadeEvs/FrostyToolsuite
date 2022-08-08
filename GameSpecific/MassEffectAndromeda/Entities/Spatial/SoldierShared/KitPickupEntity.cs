using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KitPickupEntityData))]
	public class KitPickupEntity : PickupEntity, IEntityData<FrostySdk.Ebx.KitPickupEntityData>
	{
		public new FrostySdk.Ebx.KitPickupEntityData Data => data as FrostySdk.Ebx.KitPickupEntityData;

		public KitPickupEntity(FrostySdk.Ebx.KitPickupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PickupEntityData))]
	public class PickupEntity : GamePhysicsEntity, IEntityData<FrostySdk.Ebx.PickupEntityData>
	{
		public new FrostySdk.Ebx.PickupEntityData Data => data as FrostySdk.Ebx.PickupEntityData;

		public PickupEntity(FrostySdk.Ebx.PickupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


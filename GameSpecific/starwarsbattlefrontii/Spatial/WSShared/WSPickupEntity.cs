using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSPickupEntityData))]
	public class WSPickupEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.WSPickupEntityData>
	{
		public new FrostySdk.Ebx.WSPickupEntityData Data => data as FrostySdk.Ebx.WSPickupEntityData;

		public WSPickupEntity(FrostySdk.Ebx.WSPickupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerAbilitySpatialPickupEntityData))]
	public class PlayerAbilitySpatialPickupEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.PlayerAbilitySpatialPickupEntityData>
	{
		public new FrostySdk.Ebx.PlayerAbilitySpatialPickupEntityData Data => data as FrostySdk.Ebx.PlayerAbilitySpatialPickupEntityData;

		public PlayerAbilitySpatialPickupEntity(FrostySdk.Ebx.PlayerAbilitySpatialPickupEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


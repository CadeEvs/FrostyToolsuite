using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RailRideHeliData))]
	public class RailRideHeli : GameComponentEntity, IEntityData<FrostySdk.Ebx.RailRideHeliData>
	{
		public new FrostySdk.Ebx.RailRideHeliData Data => data as FrostySdk.Ebx.RailRideHeliData;

		public RailRideHeli(FrostySdk.Ebx.RailRideHeliData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


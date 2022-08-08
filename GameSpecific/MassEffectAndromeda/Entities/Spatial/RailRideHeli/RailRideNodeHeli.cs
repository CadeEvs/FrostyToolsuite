using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RailRideNodeHeliData))]
	public class RailRideNodeHeli : GameComponentEntity, IEntityData<FrostySdk.Ebx.RailRideNodeHeliData>
	{
		public new FrostySdk.Ebx.RailRideNodeHeliData Data => data as FrostySdk.Ebx.RailRideNodeHeliData;

		public RailRideNodeHeli(FrostySdk.Ebx.RailRideNodeHeliData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


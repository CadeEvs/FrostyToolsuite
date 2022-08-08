using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RailRideNodePointToData))]
	public class RailRideNodePointTo : GameComponentEntity, IEntityData<FrostySdk.Ebx.RailRideNodePointToData>
	{
		public new FrostySdk.Ebx.RailRideNodePointToData Data => data as FrostySdk.Ebx.RailRideNodePointToData;

		public RailRideNodePointTo(FrostySdk.Ebx.RailRideNodePointToData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


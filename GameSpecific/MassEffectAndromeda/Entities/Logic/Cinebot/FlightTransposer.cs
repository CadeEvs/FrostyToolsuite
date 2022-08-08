using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FlightTransposerData))]
	public class FlightTransposer : CinebotTransposer, IEntityData<FrostySdk.Ebx.FlightTransposerData>
	{
		public new FrostySdk.Ebx.FlightTransposerData Data => data as FrostySdk.Ebx.FlightTransposerData;
		public override string DisplayName => "FlightTransposer";

		public FlightTransposer(FrostySdk.Ebx.FlightTransposerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


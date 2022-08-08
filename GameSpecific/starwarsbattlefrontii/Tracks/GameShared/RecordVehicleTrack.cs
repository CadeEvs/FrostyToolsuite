
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RecordVehicleTrackData))]
	public class RecordVehicleTrack : RecordTrackBase, IEntityData<FrostySdk.Ebx.RecordVehicleTrackData>
	{
		public new FrostySdk.Ebx.RecordVehicleTrackData Data => data as FrostySdk.Ebx.RecordVehicleTrackData;
		public override string DisplayName => "RecordVehicleTrack";

		public RecordVehicleTrack(FrostySdk.Ebx.RecordVehicleTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


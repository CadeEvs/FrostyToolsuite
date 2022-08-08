
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotControllerTrackData))]
	public class CinebotControllerTrack : CinebotTrackableTrack, IEntityData<FrostySdk.Ebx.CinebotControllerTrackData>
	{
		public new FrostySdk.Ebx.CinebotControllerTrackData Data => data as FrostySdk.Ebx.CinebotControllerTrackData;
		public override string DisplayName => "CinebotControllerTrack";

		public CinebotControllerTrack(FrostySdk.Ebx.CinebotControllerTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


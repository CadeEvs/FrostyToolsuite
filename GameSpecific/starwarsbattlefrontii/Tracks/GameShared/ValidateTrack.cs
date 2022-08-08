
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ValidateTrackData))]
	public class ValidateTrack : TimelineTrack, IEntityData<FrostySdk.Ebx.ValidateTrackData>
	{
		public new FrostySdk.Ebx.ValidateTrackData Data => data as FrostySdk.Ebx.ValidateTrackData;
		public override string DisplayName => "ValidateTrack";

		public ValidateTrack(FrostySdk.Ebx.ValidateTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


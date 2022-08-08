
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RecordRootTransformTrackData))]
	public class RecordRootTransformTrack : RecordTrackChildren, IEntityData<FrostySdk.Ebx.RecordRootTransformTrackData>
	{
		public new FrostySdk.Ebx.RecordRootTransformTrackData Data => data as FrostySdk.Ebx.RecordRootTransformTrackData;
		public override string DisplayName => "RecordRootTransformTrack";

		public RecordRootTransformTrack(FrostySdk.Ebx.RecordRootTransformTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


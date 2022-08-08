
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec3RecordPropertyTrackData))]
	public class Vec3RecordPropertyTrack : RecordPropertyTrackBase, IEntityData<FrostySdk.Ebx.Vec3RecordPropertyTrackData>
	{
		public new FrostySdk.Ebx.Vec3RecordPropertyTrackData Data => data as FrostySdk.Ebx.Vec3RecordPropertyTrackData;
		public override string DisplayName => "Vec3RecordPropertyTrack";

		public Vec3RecordPropertyTrack(FrostySdk.Ebx.Vec3RecordPropertyTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


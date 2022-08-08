
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTVec3TrackData))]
	public class ANTVec3Track : Vec3Track, IEntityData<FrostySdk.Ebx.ANTVec3TrackData>
	{
		public new FrostySdk.Ebx.ANTVec3TrackData Data => data as FrostySdk.Ebx.ANTVec3TrackData;
		public override string DisplayName => "ANTVec3Track";

		public ANTVec3Track(FrostySdk.Ebx.ANTVec3TrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


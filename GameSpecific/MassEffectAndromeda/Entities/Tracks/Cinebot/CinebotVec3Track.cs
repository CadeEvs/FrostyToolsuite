
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotVec3TrackData))]
	public class CinebotVec3Track : Vec3Track, IEntityData<FrostySdk.Ebx.CinebotVec3TrackData>
	{
		public new FrostySdk.Ebx.CinebotVec3TrackData Data => data as FrostySdk.Ebx.CinebotVec3TrackData;
		public override string DisplayName => "CinebotVec3Track";

		public CinebotVec3Track(FrostySdk.Ebx.CinebotVec3TrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


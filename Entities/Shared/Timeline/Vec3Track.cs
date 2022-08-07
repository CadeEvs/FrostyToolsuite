
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec3TrackData))]
	public class Vec3Track : PropertyTrackBase, IEntityData<FrostySdk.Ebx.Vec3TrackData>
	{
		public new FrostySdk.Ebx.Vec3TrackData Data => data as FrostySdk.Ebx.Vec3TrackData;
		public override string DisplayName => "Vec3Track";
        public override string Icon => "Images/Tracks/Vec3Track.png";

        public Vec3Track(FrostySdk.Ebx.Vec3TrackData inData, Entity inParent)
			: base(inData, inParent)
		{
			AddTrack(Data.X);
			AddTrack(Data.Y);
			AddTrack(Data.Z);
		}
	}
}


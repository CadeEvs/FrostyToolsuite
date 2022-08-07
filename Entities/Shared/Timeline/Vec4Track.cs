
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec4TrackData))]
	public class Vec4Track : PropertyTrackBase, IEntityData<FrostySdk.Ebx.Vec4TrackData>
	{
		public new FrostySdk.Ebx.Vec4TrackData Data => data as FrostySdk.Ebx.Vec4TrackData;
		public override string DisplayName => "Vec4Track";
		public override string Icon => "Images/Tracks/Vec4Track.png";

		public Vec4Track(FrostySdk.Ebx.Vec4TrackData inData, Entity inParent)
			: base(inData, inParent)
		{
			AddTrack(Data.X);
			AddTrack(Data.Y);
			AddTrack(Data.Z);
			AddTrack(Data.W);
		}
	}
}


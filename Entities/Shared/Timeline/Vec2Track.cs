
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec2TrackData))]
	public class Vec2Track : PropertyTrackBase, IEntityData<FrostySdk.Ebx.Vec2TrackData>
	{
		public new FrostySdk.Ebx.Vec2TrackData Data => data as FrostySdk.Ebx.Vec2TrackData;
		public override string DisplayName => "Vec2Track";
		public override string Icon => "Images/Tracks/Vec2Track.png";

		public Vec2Track(FrostySdk.Ebx.Vec2TrackData inData, Entity inParent)
			: base(inData, inParent)
		{
			AddTrack(Data.X);
			AddTrack(Data.Y);
		}
	}
}


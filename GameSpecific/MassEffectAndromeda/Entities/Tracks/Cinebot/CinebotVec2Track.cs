
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotVec2TrackData))]
	public class CinebotVec2Track : Vec2Track, IEntityData<FrostySdk.Ebx.CinebotVec2TrackData>
	{
		public new FrostySdk.Ebx.CinebotVec2TrackData Data => data as FrostySdk.Ebx.CinebotVec2TrackData;
		public override string DisplayName => "CinebotVec2Track";

		public CinebotVec2Track(FrostySdk.Ebx.CinebotVec2TrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}



namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotVec4TrackData))]
	public class CinebotVec4Track : Vec4Track, IEntityData<FrostySdk.Ebx.CinebotVec4TrackData>
	{
		public new FrostySdk.Ebx.CinebotVec4TrackData Data => data as FrostySdk.Ebx.CinebotVec4TrackData;
		public override string DisplayName => "CinebotVec4Track";

		public CinebotVec4Track(FrostySdk.Ebx.CinebotVec4TrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


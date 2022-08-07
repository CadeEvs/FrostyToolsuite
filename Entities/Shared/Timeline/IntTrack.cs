
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntTrackData))]
	public class IntTrack : PropertyTrackBase, IEntityData<FrostySdk.Ebx.IntTrackData>
	{
		public new FrostySdk.Ebx.IntTrackData Data => data as FrostySdk.Ebx.IntTrackData;
		public override string DisplayName => "IntTrack";

		public IntTrack(FrostySdk.Ebx.IntTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


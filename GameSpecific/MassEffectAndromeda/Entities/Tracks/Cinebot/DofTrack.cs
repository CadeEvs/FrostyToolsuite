
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DofTrackData))]
	public class DofTrack : PropertyTrackBase, IEntityData<FrostySdk.Ebx.DofTrackData>
	{
		public new FrostySdk.Ebx.DofTrackData Data => data as FrostySdk.Ebx.DofTrackData;
		public override string DisplayName => "DofTrack";
        public override string Icon => "Images/Tracks/DofTrack.png";

        public DofTrack(FrostySdk.Ebx.DofTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


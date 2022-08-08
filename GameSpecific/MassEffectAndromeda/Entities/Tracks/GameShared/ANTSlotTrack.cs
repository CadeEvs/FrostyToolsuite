
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTSlotTrackData))]
	public class ANTSlotTrack : PropertyTrackBase, IEntityData<FrostySdk.Ebx.ANTSlotTrackData>
	{
		public new FrostySdk.Ebx.ANTSlotTrackData Data => data as FrostySdk.Ebx.ANTSlotTrackData;
		public override string DisplayName => "ANTSlotTrack";
		public override string Icon => "Images/Tracks/AntTrack.png";

		public ANTSlotTrack(FrostySdk.Ebx.ANTSlotTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


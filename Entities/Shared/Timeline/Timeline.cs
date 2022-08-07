
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TimelineData))]
	public class Timeline : TimelineTrack, IEntityData<FrostySdk.Ebx.TimelineData>
	{
		public new FrostySdk.Ebx.TimelineData Data => data as FrostySdk.Ebx.TimelineData;
		public override string DisplayName => "Timeline";

		public Timeline(FrostySdk.Ebx.TimelineData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


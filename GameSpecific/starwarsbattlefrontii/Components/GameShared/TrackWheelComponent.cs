
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TrackWheelComponentData))]
	public class TrackWheelComponent : WheelComponent, IEntityData<FrostySdk.Ebx.TrackWheelComponentData>
	{
		public new FrostySdk.Ebx.TrackWheelComponentData Data => data as FrostySdk.Ebx.TrackWheelComponentData;
		public override string DisplayName => "TrackWheelComponent";

		public TrackWheelComponent(FrostySdk.Ebx.TrackWheelComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}



namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TrackComponentData))]
	public class TrackComponent : MeshComponent, IEntityData<FrostySdk.Ebx.TrackComponentData>
	{
		public new FrostySdk.Ebx.TrackComponentData Data => data as FrostySdk.Ebx.TrackComponentData;
		public override string DisplayName => "TrackComponent";

		public TrackComponent(FrostySdk.Ebx.TrackComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


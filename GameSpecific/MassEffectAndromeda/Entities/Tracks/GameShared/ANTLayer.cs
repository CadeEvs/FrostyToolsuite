
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTLayerData))]
	public class ANTLayer : TimelineTrack, IEntityData<FrostySdk.Ebx.ANTLayerData>
	{
		public new FrostySdk.Ebx.ANTLayerData Data => data as FrostySdk.Ebx.ANTLayerData;
		public override string DisplayName => "ANTLayer";
		public override string Icon => "Images/Tracks/LayerTrack.png";

		public ANTLayer(FrostySdk.Ebx.ANTLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}



namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ANTControllerLayerData))]
	public class ANTControllerLayer : ANTLayer, IEntityData<FrostySdk.Ebx.ANTControllerLayerData>
	{
		public new FrostySdk.Ebx.ANTControllerLayerData Data => data as FrostySdk.Ebx.ANTControllerLayerData;
		public override string DisplayName => "ANTControllerLayer";

        public ANTControllerLayer(FrostySdk.Ebx.ANTControllerLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


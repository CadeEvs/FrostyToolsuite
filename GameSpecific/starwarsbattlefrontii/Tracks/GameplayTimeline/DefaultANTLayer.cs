
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DefaultANTLayerData))]
	public class DefaultANTLayer : ANTLayer, IEntityData<FrostySdk.Ebx.DefaultANTLayerData>
	{
		public new FrostySdk.Ebx.DefaultANTLayerData Data => data as FrostySdk.Ebx.DefaultANTLayerData;
		public override string DisplayName => "DefaultANTLayer";

		public DefaultANTLayer(FrostySdk.Ebx.DefaultANTLayerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


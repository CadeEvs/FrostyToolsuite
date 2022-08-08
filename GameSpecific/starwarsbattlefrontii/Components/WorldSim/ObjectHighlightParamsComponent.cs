
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectHighlightParamsComponentData))]
	public class ObjectHighlightParamsComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.ObjectHighlightParamsComponentData>
	{
		public new FrostySdk.Ebx.ObjectHighlightParamsComponentData Data => data as FrostySdk.Ebx.ObjectHighlightParamsComponentData;
		public override string DisplayName => "ObjectHighlightParamsComponent";

		public ObjectHighlightParamsComponent(FrostySdk.Ebx.ObjectHighlightParamsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


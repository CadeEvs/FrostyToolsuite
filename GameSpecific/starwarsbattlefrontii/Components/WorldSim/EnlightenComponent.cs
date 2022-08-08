
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnlightenComponentData))]
	public class EnlightenComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.EnlightenComponentData>
	{
		public new FrostySdk.Ebx.EnlightenComponentData Data => data as FrostySdk.Ebx.EnlightenComponentData;
		public override string DisplayName => "EnlightenComponent";

		public EnlightenComponent(FrostySdk.Ebx.EnlightenComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


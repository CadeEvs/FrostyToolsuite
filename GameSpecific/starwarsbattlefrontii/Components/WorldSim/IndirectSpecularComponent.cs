
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IndirectSpecularComponentData))]
	public class IndirectSpecularComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.IndirectSpecularComponentData>
	{
		public new FrostySdk.Ebx.IndirectSpecularComponentData Data => data as FrostySdk.Ebx.IndirectSpecularComponentData;
		public override string DisplayName => "IndirectSpecularComponent";

		public IndirectSpecularComponent(FrostySdk.Ebx.IndirectSpecularComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


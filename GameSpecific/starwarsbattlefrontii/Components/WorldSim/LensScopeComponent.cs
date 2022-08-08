
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LensScopeComponentData))]
	public class LensScopeComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.LensScopeComponentData>
	{
		public new FrostySdk.Ebx.LensScopeComponentData Data => data as FrostySdk.Ebx.LensScopeComponentData;
		public override string DisplayName => "LensScopeComponent";

		public LensScopeComponent(FrostySdk.Ebx.LensScopeComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


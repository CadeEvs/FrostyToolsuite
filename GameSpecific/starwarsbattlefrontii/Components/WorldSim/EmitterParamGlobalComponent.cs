
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EmitterParamGlobalComponentData))]
	public class EmitterParamGlobalComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.EmitterParamGlobalComponentData>
	{
		public new FrostySdk.Ebx.EmitterParamGlobalComponentData Data => data as FrostySdk.Ebx.EmitterParamGlobalComponentData;
		public override string DisplayName => "EmitterParamGlobalComponent";

		public EmitterParamGlobalComponent(FrostySdk.Ebx.EmitterParamGlobalComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


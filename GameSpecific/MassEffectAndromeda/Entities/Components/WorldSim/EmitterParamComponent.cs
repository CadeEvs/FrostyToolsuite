using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EmitterParamComponentData))]
	public class EmitterParamComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.EmitterParamComponentData>
	{
		public new FrostySdk.Ebx.EmitterParamComponentData Data => data as FrostySdk.Ebx.EmitterParamComponentData;
		public override string DisplayName => "EmitterParamComponent";

		public EmitterParamComponent(FrostySdk.Ebx.EmitterParamComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


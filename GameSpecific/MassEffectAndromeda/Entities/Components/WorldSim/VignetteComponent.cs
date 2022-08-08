using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VignetteComponentData))]
	public class VignetteComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.VignetteComponentData>
	{
		public new FrostySdk.Ebx.VignetteComponentData Data => data as FrostySdk.Ebx.VignetteComponentData;
		public override string DisplayName => "VignetteComponent";

		public VignetteComponent(FrostySdk.Ebx.VignetteComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


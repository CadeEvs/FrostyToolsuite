using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VisualEnvironmentComponentData))]
	public class VisualEnvironmentComponent : Component, IEntityData<FrostySdk.Ebx.VisualEnvironmentComponentData>
	{
		public new FrostySdk.Ebx.VisualEnvironmentComponentData Data => data as FrostySdk.Ebx.VisualEnvironmentComponentData;
		public override string DisplayName => "VisualEnvironmentComponent";

		public VisualEnvironmentComponent(FrostySdk.Ebx.VisualEnvironmentComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


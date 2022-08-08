using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebugComponentData))]
	public class DebugComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.DebugComponentData>
	{
		public new FrostySdk.Ebx.DebugComponentData Data => data as FrostySdk.Ebx.DebugComponentData;
		public override string DisplayName => "DebugComponent";

		public DebugComponent(FrostySdk.Ebx.DebugComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


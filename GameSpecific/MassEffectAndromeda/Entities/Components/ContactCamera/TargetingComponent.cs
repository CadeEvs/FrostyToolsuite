using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetingComponentData))]
	public class TargetingComponent : GameComponent, IEntityData<FrostySdk.Ebx.TargetingComponentData>
	{
		public new FrostySdk.Ebx.TargetingComponentData Data => data as FrostySdk.Ebx.TargetingComponentData;
		public override string DisplayName => "TargetingComponent";

		public TargetingComponent(FrostySdk.Ebx.TargetingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


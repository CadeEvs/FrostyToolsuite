using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpaceMiningNodeComponentData))]
	public class SpaceMiningNodeComponent : GameComponent, IEntityData<FrostySdk.Ebx.SpaceMiningNodeComponentData>
	{
		public new FrostySdk.Ebx.SpaceMiningNodeComponentData Data => data as FrostySdk.Ebx.SpaceMiningNodeComponentData;
		public override string DisplayName => "SpaceMiningNodeComponent";

		public SpaceMiningNodeComponent(FrostySdk.Ebx.SpaceMiningNodeComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


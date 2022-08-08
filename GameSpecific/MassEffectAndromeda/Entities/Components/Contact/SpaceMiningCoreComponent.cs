using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpaceMiningCoreComponentData))]
	public class SpaceMiningCoreComponent : GameComponent, IEntityData<FrostySdk.Ebx.SpaceMiningCoreComponentData>
	{
		public new FrostySdk.Ebx.SpaceMiningCoreComponentData Data => data as FrostySdk.Ebx.SpaceMiningCoreComponentData;
		public override string DisplayName => "SpaceMiningCoreComponent";

		public SpaceMiningCoreComponent(FrostySdk.Ebx.SpaceMiningCoreComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


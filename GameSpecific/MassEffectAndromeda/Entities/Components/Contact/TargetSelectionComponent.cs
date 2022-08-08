using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetSelectionComponentData))]
	public class TargetSelectionComponent : GameComponent, IEntityData<FrostySdk.Ebx.TargetSelectionComponentData>
	{
		public new FrostySdk.Ebx.TargetSelectionComponentData Data => data as FrostySdk.Ebx.TargetSelectionComponentData;
		public override string DisplayName => "TargetSelectionComponent";

		public TargetSelectionComponent(FrostySdk.Ebx.TargetSelectionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


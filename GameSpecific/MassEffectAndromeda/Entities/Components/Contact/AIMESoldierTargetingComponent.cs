using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIMESoldierTargetingComponentData))]
	public class AIMESoldierTargetingComponent : MESoldierTargetingComponent, IEntityData<FrostySdk.Ebx.AIMESoldierTargetingComponentData>
	{
		public new FrostySdk.Ebx.AIMESoldierTargetingComponentData Data => data as FrostySdk.Ebx.AIMESoldierTargetingComponentData;
		public override string DisplayName => "AIMESoldierTargetingComponent";

		public AIMESoldierTargetingComponent(FrostySdk.Ebx.AIMESoldierTargetingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


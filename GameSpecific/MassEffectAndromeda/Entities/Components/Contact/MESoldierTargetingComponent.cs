using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MESoldierTargetingComponentData))]
	public class MESoldierTargetingComponent : TargetingComponent, IEntityData<FrostySdk.Ebx.MESoldierTargetingComponentData>
	{
		public new FrostySdk.Ebx.MESoldierTargetingComponentData Data => data as FrostySdk.Ebx.MESoldierTargetingComponentData;
		public override string DisplayName => "MESoldierTargetingComponent";

		public MESoldierTargetingComponent(FrostySdk.Ebx.MESoldierTargetingComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


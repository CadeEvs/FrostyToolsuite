using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MESoldierMovementComponentData))]
	public class MESoldierMovementComponent : MovementComponent, IEntityData<FrostySdk.Ebx.MESoldierMovementComponentData>
	{
		public new FrostySdk.Ebx.MESoldierMovementComponentData Data => data as FrostySdk.Ebx.MESoldierMovementComponentData;
		public override string DisplayName => "MESoldierMovementComponent";

		public MESoldierMovementComponent(FrostySdk.Ebx.MESoldierMovementComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


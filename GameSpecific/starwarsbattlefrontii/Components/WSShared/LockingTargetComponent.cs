
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LockingTargetComponentData))]
	public class LockingTargetComponent : GameComponent, IEntityData<FrostySdk.Ebx.LockingTargetComponentData>
	{
		public new FrostySdk.Ebx.LockingTargetComponentData Data => data as FrostySdk.Ebx.LockingTargetComponentData;
		public override string DisplayName => "LockingTargetComponent";

		public LockingTargetComponent(FrostySdk.Ebx.LockingTargetComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}



namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoftLockHintComponentData))]
	public class SoftLockHintComponent : GameComponent, IEntityData<FrostySdk.Ebx.SoftLockHintComponentData>
	{
		public new FrostySdk.Ebx.SoftLockHintComponentData Data => data as FrostySdk.Ebx.SoftLockHintComponentData;
		public override string DisplayName => "SoftLockHintComponent";

		public SoftLockHintComponent(FrostySdk.Ebx.SoftLockHintComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


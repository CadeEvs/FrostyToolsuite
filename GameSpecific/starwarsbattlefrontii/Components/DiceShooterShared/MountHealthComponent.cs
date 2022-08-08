
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MountHealthComponentData))]
	public class MountHealthComponent : ControllableHealthComponent, IEntityData<FrostySdk.Ebx.MountHealthComponentData>
	{
		public new FrostySdk.Ebx.MountHealthComponentData Data => data as FrostySdk.Ebx.MountHealthComponentData;
		public override string DisplayName => "MountHealthComponent";

		public MountHealthComponent(FrostySdk.Ebx.MountHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


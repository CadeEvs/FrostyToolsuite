
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSMountHealthComponentData))]
	public class WSMountHealthComponent : MountHealthComponent, IEntityData<FrostySdk.Ebx.WSMountHealthComponentData>
	{
		public new FrostySdk.Ebx.WSMountHealthComponentData Data => data as FrostySdk.Ebx.WSMountHealthComponentData;
		public override string DisplayName => "WSMountHealthComponent";

		public WSMountHealthComponent(FrostySdk.Ebx.WSMountHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


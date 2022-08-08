
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BangerHealthComponentData))]
	public class BangerHealthComponent : GameHealthComponent, IEntityData<FrostySdk.Ebx.BangerHealthComponentData>
	{
		public new FrostySdk.Ebx.BangerHealthComponentData Data => data as FrostySdk.Ebx.BangerHealthComponentData;
		public override string DisplayName => "BangerHealthComponent";

		public BangerHealthComponent(FrostySdk.Ebx.BangerHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


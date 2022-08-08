
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShieldComponentData))]
	public class ShieldComponent : GameComponent, IEntityData<FrostySdk.Ebx.ShieldComponentData>
	{
		public new FrostySdk.Ebx.ShieldComponentData Data => data as FrostySdk.Ebx.ShieldComponentData;
		public override string DisplayName => "ShieldComponent";

		public ShieldComponent(FrostySdk.Ebx.ShieldComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}



namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShieldControllerComponentData))]
	public class ShieldControllerComponent : GameComponent, IEntityData<FrostySdk.Ebx.ShieldControllerComponentData>
	{
		public new FrostySdk.Ebx.ShieldControllerComponentData Data => data as FrostySdk.Ebx.ShieldControllerComponentData;
		public override string DisplayName => "ShieldControllerComponent";

		public ShieldControllerComponent(FrostySdk.Ebx.ShieldControllerComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}



namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShieldStateComponentData))]
	public class ShieldStateComponent : GameComponent, IEntityData<FrostySdk.Ebx.ShieldStateComponentData>
	{
		public new FrostySdk.Ebx.ShieldStateComponentData Data => data as FrostySdk.Ebx.ShieldStateComponentData;
		public override string DisplayName => "ShieldStateComponent";

		public ShieldStateComponent(FrostySdk.Ebx.ShieldStateComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


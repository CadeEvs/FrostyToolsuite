
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIProjectileComponentData))]
	public class AIProjectileComponent : GameComponent, IEntityData<FrostySdk.Ebx.AIProjectileComponentData>
	{
		public new FrostySdk.Ebx.AIProjectileComponentData Data => data as FrostySdk.Ebx.AIProjectileComponentData;
		public override string DisplayName => "AIProjectileComponent";

		public AIProjectileComponent(FrostySdk.Ebx.AIProjectileComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


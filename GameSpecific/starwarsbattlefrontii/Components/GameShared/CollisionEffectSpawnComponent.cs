
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CollisionEffectSpawnComponentData))]
	public class CollisionEffectSpawnComponent : GameComponent, IEntityData<FrostySdk.Ebx.CollisionEffectSpawnComponentData>
	{
		public new FrostySdk.Ebx.CollisionEffectSpawnComponentData Data => data as FrostySdk.Ebx.CollisionEffectSpawnComponentData;
		public override string DisplayName => "CollisionEffectSpawnComponent";

		public CollisionEffectSpawnComponent(FrostySdk.Ebx.CollisionEffectSpawnComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}



namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticModelGroupPhysicsComponentData))]
	public class StaticModelGroupPhysicsComponent : GamePhysicsComponent, IEntityData<FrostySdk.Ebx.StaticModelGroupPhysicsComponentData>
	{
		public new FrostySdk.Ebx.StaticModelGroupPhysicsComponentData Data => data as FrostySdk.Ebx.StaticModelGroupPhysicsComponentData;
		public override string DisplayName => "StaticModelGroupPhysicsComponent";

		public StaticModelGroupPhysicsComponent(FrostySdk.Ebx.StaticModelGroupPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


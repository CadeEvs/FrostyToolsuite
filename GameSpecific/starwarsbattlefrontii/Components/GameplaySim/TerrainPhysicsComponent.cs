
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TerrainPhysicsComponentData))]
	public class TerrainPhysicsComponent : GamePhysicsComponent, IEntityData<FrostySdk.Ebx.TerrainPhysicsComponentData>
	{
		public new FrostySdk.Ebx.TerrainPhysicsComponentData Data => data as FrostySdk.Ebx.TerrainPhysicsComponentData;
		public override string DisplayName => "TerrainPhysicsComponent";

		public TerrainPhysicsComponent(FrostySdk.Ebx.TerrainPhysicsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


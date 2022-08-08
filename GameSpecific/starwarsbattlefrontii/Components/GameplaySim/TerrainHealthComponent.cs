
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TerrainHealthComponentData))]
	public class TerrainHealthComponent : GameHealthComponent, IEntityData<FrostySdk.Ebx.TerrainHealthComponentData>
	{
		public new FrostySdk.Ebx.TerrainHealthComponentData Data => data as FrostySdk.Ebx.TerrainHealthComponentData;
		public override string DisplayName => "TerrainHealthComponent";

		public TerrainHealthComponent(FrostySdk.Ebx.TerrainHealthComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}



namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TerrainDecalComponentData))]
	public class TerrainDecalComponent : GameComponent, IEntityData<FrostySdk.Ebx.TerrainDecalComponentData>
	{
		public new FrostySdk.Ebx.TerrainDecalComponentData Data => data as FrostySdk.Ebx.TerrainDecalComponentData;
		public override string DisplayName => "TerrainDecalComponent";

		public TerrainDecalComponent(FrostySdk.Ebx.TerrainDecalComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


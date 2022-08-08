
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DebrisClusterSoundsComponentData))]
	public class DebrisClusterSoundsComponent : GameComponent, IEntityData<FrostySdk.Ebx.DebrisClusterSoundsComponentData>
	{
		public new FrostySdk.Ebx.DebrisClusterSoundsComponentData Data => data as FrostySdk.Ebx.DebrisClusterSoundsComponentData;
		public override string DisplayName => "DebrisClusterSoundsComponent";

		public DebrisClusterSoundsComponent(FrostySdk.Ebx.DebrisClusterSoundsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


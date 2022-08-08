
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EdgeModelComponentData))]
	public class EdgeModelComponent : GameComponent, IEntityData<FrostySdk.Ebx.EdgeModelComponentData>
	{
		public new FrostySdk.Ebx.EdgeModelComponentData Data => data as FrostySdk.Ebx.EdgeModelComponentData;
		public override string DisplayName => "EdgeModelComponent";

		public EdgeModelComponent(FrostySdk.Ebx.EdgeModelComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


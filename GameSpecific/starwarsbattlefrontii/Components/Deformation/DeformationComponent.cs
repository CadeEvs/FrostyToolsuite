
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DeformationComponentData))]
	public class DeformationComponent : GameComponent, IEntityData<FrostySdk.Ebx.DeformationComponentData>
	{
		public new FrostySdk.Ebx.DeformationComponentData Data => data as FrostySdk.Ebx.DeformationComponentData;
		public override string DisplayName => "DeformationComponent";

		public DeformationComponent(FrostySdk.Ebx.DeformationComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


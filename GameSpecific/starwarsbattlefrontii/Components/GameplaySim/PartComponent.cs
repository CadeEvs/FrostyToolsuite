
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PartComponentData))]
	public class PartComponent : GameComponent, IEntityData<FrostySdk.Ebx.PartComponentData>
	{
		public new FrostySdk.Ebx.PartComponentData Data => data as FrostySdk.Ebx.PartComponentData;
		public override string DisplayName => "PartComponent";

		public PartComponent(FrostySdk.Ebx.PartComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


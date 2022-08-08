
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIMeleeComponentData))]
	public class AIMeleeComponent : GameComponent, IEntityData<FrostySdk.Ebx.AIMeleeComponentData>
	{
		public new FrostySdk.Ebx.AIMeleeComponentData Data => data as FrostySdk.Ebx.AIMeleeComponentData;
		public override string DisplayName => "AIMeleeComponent";

		public AIMeleeComponent(FrostySdk.Ebx.AIMeleeComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


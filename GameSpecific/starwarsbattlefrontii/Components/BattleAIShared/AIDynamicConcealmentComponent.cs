
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIDynamicConcealmentComponentData))]
	public class AIDynamicConcealmentComponent : GameComponent, IEntityData<FrostySdk.Ebx.AIDynamicConcealmentComponentData>
	{
		public new FrostySdk.Ebx.AIDynamicConcealmentComponentData Data => data as FrostySdk.Ebx.AIDynamicConcealmentComponentData;
		public override string DisplayName => "AIDynamicConcealmentComponent";

		public AIDynamicConcealmentComponent(FrostySdk.Ebx.AIDynamicConcealmentComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


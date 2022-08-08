
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIAnchorToPointComponentData))]
	public class AIAnchorToPointComponent : GameComponent, IEntityData<FrostySdk.Ebx.AIAnchorToPointComponentData>
	{
		public new FrostySdk.Ebx.AIAnchorToPointComponentData Data => data as FrostySdk.Ebx.AIAnchorToPointComponentData;
		public override string DisplayName => "AIAnchorToPointComponent";

		public AIAnchorToPointComponent(FrostySdk.Ebx.AIAnchorToPointComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AIBlackboardComponentData))]
	public class AIBlackboardComponent : BlackboardComponent, IEntityData<FrostySdk.Ebx.AIBlackboardComponentData>
	{
		public new FrostySdk.Ebx.AIBlackboardComponentData Data => data as FrostySdk.Ebx.AIBlackboardComponentData;
		public override string DisplayName => "AIBlackboardComponent";

		public AIBlackboardComponent(FrostySdk.Ebx.AIBlackboardComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BlackboardComponentData))]
	public class BlackboardComponent : GameComponent, IEntityData<FrostySdk.Ebx.BlackboardComponentData>
	{
		public new FrostySdk.Ebx.BlackboardComponentData Data => data as FrostySdk.Ebx.BlackboardComponentData;
		public override string DisplayName => "BlackboardComponent";

		public BlackboardComponent(FrostySdk.Ebx.BlackboardComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


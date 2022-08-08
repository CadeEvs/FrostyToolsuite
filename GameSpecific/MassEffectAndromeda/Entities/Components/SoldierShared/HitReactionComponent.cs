using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HitReactionComponentData))]
	public class HitReactionComponent : GameComponent, IEntityData<FrostySdk.Ebx.HitReactionComponentData>
	{
		public new FrostySdk.Ebx.HitReactionComponentData Data => data as FrostySdk.Ebx.HitReactionComponentData;
		public override string DisplayName => "HitReactionComponent";

		public HitReactionComponent(FrostySdk.Ebx.HitReactionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


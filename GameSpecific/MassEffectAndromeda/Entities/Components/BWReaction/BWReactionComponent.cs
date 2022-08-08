using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWReactionComponentData))]
	public class BWReactionComponent : GameComponent, IEntityData<FrostySdk.Ebx.BWReactionComponentData>
	{
		public new FrostySdk.Ebx.BWReactionComponentData Data => data as FrostySdk.Ebx.BWReactionComponentData;
		public override string DisplayName => "BWReactionComponent";

		public BWReactionComponent(FrostySdk.Ebx.BWReactionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


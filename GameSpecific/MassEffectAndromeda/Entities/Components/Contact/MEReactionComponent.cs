using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEReactionComponentData))]
	public class MEReactionComponent : BWReactionComponent, IEntityData<FrostySdk.Ebx.MEReactionComponentData>
	{
		public new FrostySdk.Ebx.MEReactionComponentData Data => data as FrostySdk.Ebx.MEReactionComponentData;
		public override string DisplayName => "MEReactionComponent";

		public MEReactionComponent(FrostySdk.Ebx.MEReactionComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


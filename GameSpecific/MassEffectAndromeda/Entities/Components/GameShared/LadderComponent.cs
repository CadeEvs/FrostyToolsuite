using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LadderComponentData))]
	public class LadderComponent : GameComponent, IEntityData<FrostySdk.Ebx.LadderComponentData>
	{
		public new FrostySdk.Ebx.LadderComponentData Data => data as FrostySdk.Ebx.LadderComponentData;
		public override string DisplayName => "LadderComponent";

		public LadderComponent(FrostySdk.Ebx.LadderComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


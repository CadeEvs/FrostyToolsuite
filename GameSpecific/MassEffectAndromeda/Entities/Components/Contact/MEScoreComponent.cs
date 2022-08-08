using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEScoreComponentData))]
	public class MEScoreComponent : GameComponent, IEntityData<FrostySdk.Ebx.MEScoreComponentData>
	{
		public new FrostySdk.Ebx.MEScoreComponentData Data => data as FrostySdk.Ebx.MEScoreComponentData;
		public override string DisplayName => "MEScoreComponent";

		public MEScoreComponent(FrostySdk.Ebx.MEScoreComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


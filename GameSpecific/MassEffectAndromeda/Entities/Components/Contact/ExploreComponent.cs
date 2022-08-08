using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExploreComponentData))]
	public class ExploreComponent : GameComponent, IEntityData<FrostySdk.Ebx.ExploreComponentData>
	{
		public new FrostySdk.Ebx.ExploreComponentData Data => data as FrostySdk.Ebx.ExploreComponentData;
		public override string DisplayName => "ExploreComponent";

		public ExploreComponent(FrostySdk.Ebx.ExploreComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


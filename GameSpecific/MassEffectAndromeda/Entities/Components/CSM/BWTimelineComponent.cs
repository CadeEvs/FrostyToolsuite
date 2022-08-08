using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWTimelineComponentData))]
	public class BWTimelineComponent : GameComponent, IEntityData<FrostySdk.Ebx.BWTimelineComponentData>
	{
		public new FrostySdk.Ebx.BWTimelineComponentData Data => data as FrostySdk.Ebx.BWTimelineComponentData;
		public override string DisplayName => "BWTimelineComponent";

		public BWTimelineComponent(FrostySdk.Ebx.BWTimelineComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


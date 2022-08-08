using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWStatsComponentData))]
	public class BWStatsComponent : GameComponent, IEntityData<FrostySdk.Ebx.BWStatsComponentData>
	{
		public new FrostySdk.Ebx.BWStatsComponentData Data => data as FrostySdk.Ebx.BWStatsComponentData;
		public override string DisplayName => "BWStatsComponent";

		public BWStatsComponent(FrostySdk.Ebx.BWStatsComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


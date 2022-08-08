using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpaceTravelComponentData))]
	public class SpaceTravelComponent : GameComponent, IEntityData<FrostySdk.Ebx.SpaceTravelComponentData>
	{
		public new FrostySdk.Ebx.SpaceTravelComponentData Data => data as FrostySdk.Ebx.SpaceTravelComponentData;
		public override string DisplayName => "SpaceTravelComponent";

		public SpaceTravelComponent(FrostySdk.Ebx.SpaceTravelComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


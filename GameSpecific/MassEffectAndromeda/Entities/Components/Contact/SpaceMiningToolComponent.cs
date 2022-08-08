using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpaceMiningToolComponentData))]
	public class SpaceMiningToolComponent : GameComponent, IEntityData<FrostySdk.Ebx.SpaceMiningToolComponentData>
	{
		public new FrostySdk.Ebx.SpaceMiningToolComponentData Data => data as FrostySdk.Ebx.SpaceMiningToolComponentData;
		public override string DisplayName => "SpaceMiningToolComponent";

		public SpaceMiningToolComponent(FrostySdk.Ebx.SpaceMiningToolComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


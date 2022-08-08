using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AdvancedMiningComponentData))]
	public class AdvancedMiningComponent : GameComponent, IEntityData<FrostySdk.Ebx.AdvancedMiningComponentData>
	{
		public new FrostySdk.Ebx.AdvancedMiningComponentData Data => data as FrostySdk.Ebx.AdvancedMiningComponentData;
		public override string DisplayName => "AdvancedMiningComponent";

		public AdvancedMiningComponent(FrostySdk.Ebx.AdvancedMiningComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


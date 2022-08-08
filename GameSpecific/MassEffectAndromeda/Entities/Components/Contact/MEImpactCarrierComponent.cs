using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEImpactCarrierComponentData))]
	public class MEImpactCarrierComponent : GameComponent, IEntityData<FrostySdk.Ebx.MEImpactCarrierComponentData>
	{
		public new FrostySdk.Ebx.MEImpactCarrierComponentData Data => data as FrostySdk.Ebx.MEImpactCarrierComponentData;
		public override string DisplayName => "MEImpactCarrierComponent";

		public MEImpactCarrierComponent(FrostySdk.Ebx.MEImpactCarrierComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


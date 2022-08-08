using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ReconProbeComponentData))]
	public class ReconProbeComponent : GameComponent, IEntityData<FrostySdk.Ebx.ReconProbeComponentData>
	{
		public new FrostySdk.Ebx.ReconProbeComponentData Data => data as FrostySdk.Ebx.ReconProbeComponentData;
		public override string DisplayName => "ReconProbeComponent";

		public ReconProbeComponent(FrostySdk.Ebx.ReconProbeComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


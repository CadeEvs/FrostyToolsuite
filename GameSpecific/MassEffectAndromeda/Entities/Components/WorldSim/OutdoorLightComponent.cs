using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OutdoorLightComponentData))]
	public class OutdoorLightComponent : VisualEnvironmentComponent, IEntityData<FrostySdk.Ebx.OutdoorLightComponentData>
	{
		public new FrostySdk.Ebx.OutdoorLightComponentData Data => data as FrostySdk.Ebx.OutdoorLightComponentData;
		public override string DisplayName => "OutdoorLightComponent";

		public OutdoorLightComponent(FrostySdk.Ebx.OutdoorLightComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


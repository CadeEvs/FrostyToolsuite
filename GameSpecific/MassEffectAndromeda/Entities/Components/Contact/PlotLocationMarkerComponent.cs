using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotLocationMarkerComponentData))]
	public class PlotLocationMarkerComponent : LocationMarkerComponent, IEntityData<FrostySdk.Ebx.PlotLocationMarkerComponentData>
	{
		public new FrostySdk.Ebx.PlotLocationMarkerComponentData Data => data as FrostySdk.Ebx.PlotLocationMarkerComponentData;
		public override string DisplayName => "PlotLocationMarkerComponent";

		public PlotLocationMarkerComponent(FrostySdk.Ebx.PlotLocationMarkerComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


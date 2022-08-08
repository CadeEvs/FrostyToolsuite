using Frosty.Core.Viewport;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocationMarkerComponentData))]
	public class LocationMarkerComponent : GameComponent, IEntityData<FrostySdk.Ebx.LocationMarkerComponentData>
	{
		public new FrostySdk.Ebx.LocationMarkerComponentData Data => data as FrostySdk.Ebx.LocationMarkerComponentData;
		public override string DisplayName => "LocationMarkerComponent";

		public LocationMarkerComponent(FrostySdk.Ebx.LocationMarkerComponentData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


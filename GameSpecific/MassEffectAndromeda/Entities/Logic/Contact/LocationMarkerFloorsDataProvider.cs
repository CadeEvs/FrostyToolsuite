using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocationMarkerFloorsDataProviderData))]
	public class LocationMarkerFloorsDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.LocationMarkerFloorsDataProviderData>
	{
		public new FrostySdk.Ebx.LocationMarkerFloorsDataProviderData Data => data as FrostySdk.Ebx.LocationMarkerFloorsDataProviderData;
		public override string DisplayName => "LocationMarkerFloorsDataProvider";

		public LocationMarkerFloorsDataProvider(FrostySdk.Ebx.LocationMarkerFloorsDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocationMarkerFilterDataProviderData))]
	public class LocationMarkerFilterDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.LocationMarkerFilterDataProviderData>
	{
		public new FrostySdk.Ebx.LocationMarkerFilterDataProviderData Data => data as FrostySdk.Ebx.LocationMarkerFilterDataProviderData;
		public override string DisplayName => "LocationMarkerFilterDataProvider";

		public LocationMarkerFilterDataProvider(FrostySdk.Ebx.LocationMarkerFilterDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


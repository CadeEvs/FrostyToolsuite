using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocationMarkerManagerEntityData))]
	public class LocationMarkerManagerEntity : SingletonEntity, IEntityData<FrostySdk.Ebx.LocationMarkerManagerEntityData>
	{
		public new FrostySdk.Ebx.LocationMarkerManagerEntityData Data => data as FrostySdk.Ebx.LocationMarkerManagerEntityData;
		public override string DisplayName => "LocationMarkerManager";

		public LocationMarkerManagerEntity(FrostySdk.Ebx.LocationMarkerManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocationMarkerEntityData))]
	public class LocationMarkerEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.LocationMarkerEntityData>
	{
		public new FrostySdk.Ebx.LocationMarkerEntityData Data => data as FrostySdk.Ebx.LocationMarkerEntityData;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Visible", Direction.In)
			};
		}

        public LocationMarkerEntity(FrostySdk.Ebx.LocationMarkerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


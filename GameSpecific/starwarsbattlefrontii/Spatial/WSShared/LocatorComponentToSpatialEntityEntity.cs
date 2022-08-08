using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocatorComponentToSpatialEntityEntityData))]
	public class LocatorComponentToSpatialEntityEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.LocatorComponentToSpatialEntityEntityData>
	{
		public new FrostySdk.Ebx.LocatorComponentToSpatialEntityEntityData Data => data as FrostySdk.Ebx.LocatorComponentToSpatialEntityEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LocatorComponentToSpatialEntityEntity(FrostySdk.Ebx.LocatorComponentToSpatialEntityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DistanceEventEntityData))]
	public class DistanceEventEntity : SimpleEntity, IEntityData<FrostySdk.Ebx.DistanceEventEntityData>
	{
		public new FrostySdk.Ebx.DistanceEventEntityData Data => data as FrostySdk.Ebx.DistanceEventEntityData;
		public override string DisplayName => "DistanceEvent";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DistanceEventEntity(FrostySdk.Ebx.DistanceEventEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


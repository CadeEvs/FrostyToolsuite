using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayPenZoneEntityData))]
	public class PlayPenZoneEntity : ZoneEntity, IEntityData<FrostySdk.Ebx.PlayPenZoneEntityData>
	{
		public new FrostySdk.Ebx.PlayPenZoneEntityData Data => data as FrostySdk.Ebx.PlayPenZoneEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PlayPenZoneEntity(FrostySdk.Ebx.PlayPenZoneEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


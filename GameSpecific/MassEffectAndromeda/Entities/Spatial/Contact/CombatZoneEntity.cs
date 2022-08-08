using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CombatZoneEntityData))]
	public class CombatZoneEntity : ZoneEntity, IEntityData<FrostySdk.Ebx.CombatZoneEntityData>
	{
		public new FrostySdk.Ebx.CombatZoneEntityData Data => data as FrostySdk.Ebx.CombatZoneEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CombatZoneEntity(FrostySdk.Ebx.CombatZoneEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DangerZoneManagerEntityData))]
	public class DangerZoneManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DangerZoneManagerEntityData>
	{
		public new FrostySdk.Ebx.DangerZoneManagerEntityData Data => data as FrostySdk.Ebx.DangerZoneManagerEntityData;
		public override string DisplayName => "DangerZoneManager";

		public DangerZoneManagerEntity(FrostySdk.Ebx.DangerZoneManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


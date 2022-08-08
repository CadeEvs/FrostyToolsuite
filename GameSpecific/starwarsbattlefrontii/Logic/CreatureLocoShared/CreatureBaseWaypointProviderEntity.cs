using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreatureBaseWaypointProviderEntityData))]
	public class CreatureBaseWaypointProviderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CreatureBaseWaypointProviderEntityData>
	{
		public new FrostySdk.Ebx.CreatureBaseWaypointProviderEntityData Data => data as FrostySdk.Ebx.CreatureBaseWaypointProviderEntityData;
		public override string DisplayName => "CreatureBaseWaypointProvider";

		public CreatureBaseWaypointProviderEntity(FrostySdk.Ebx.CreatureBaseWaypointProviderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


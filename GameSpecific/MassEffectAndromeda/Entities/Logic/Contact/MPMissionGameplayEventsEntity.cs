using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MPMissionGameplayEventsEntityData))]
	public class MPMissionGameplayEventsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MPMissionGameplayEventsEntityData>
	{
		public new FrostySdk.Ebx.MPMissionGameplayEventsEntityData Data => data as FrostySdk.Ebx.MPMissionGameplayEventsEntityData;
		public override string DisplayName => "MPMissionGameplayEvents";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MPMissionGameplayEventsEntity(FrostySdk.Ebx.MPMissionGameplayEventsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotConditionNotificationEntityData))]
	public class PlotConditionNotificationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlotConditionNotificationEntityData>
	{
		public new FrostySdk.Ebx.PlotConditionNotificationEntityData Data => data as FrostySdk.Ebx.PlotConditionNotificationEntityData;
		public override string DisplayName => "PlotConditionNotification";

		public PlotConditionNotificationEntity(FrostySdk.Ebx.PlotConditionNotificationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


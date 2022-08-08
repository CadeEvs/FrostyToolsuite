using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotIntegerFlagNotificationEntityData))]
	public class PlotIntegerFlagNotificationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlotIntegerFlagNotificationEntityData>
	{
		public new FrostySdk.Ebx.PlotIntegerFlagNotificationEntityData Data => data as FrostySdk.Ebx.PlotIntegerFlagNotificationEntityData;
		public override string DisplayName => "PlotIntegerFlagNotification";

		public PlotIntegerFlagNotificationEntity(FrostySdk.Ebx.PlotIntegerFlagNotificationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


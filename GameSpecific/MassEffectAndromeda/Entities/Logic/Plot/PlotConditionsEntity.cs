using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotConditionsEntityData))]
	public class PlotConditionsEntity : PlotConditionNotificationEntity, IEntityData<FrostySdk.Ebx.PlotConditionsEntityData>
	{
		public new FrostySdk.Ebx.PlotConditionsEntityData Data => data as FrostySdk.Ebx.PlotConditionsEntityData;
		public override string DisplayName => "PlotConditions";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Check", Direction.In),
				new ConnectionDesc("OnTrue", Direction.Out),
				new ConnectionDesc("OnFalse", Direction.Out)
			};
		}

		public PlotConditionsEntity(FrostySdk.Ebx.PlotConditionsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


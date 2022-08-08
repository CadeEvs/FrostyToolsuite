using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotConditionEntityData))]
	public class PlotConditionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlotConditionEntityData>
	{
		public new FrostySdk.Ebx.PlotConditionEntityData Data => data as FrostySdk.Ebx.PlotConditionEntityData;
		public override string DisplayName => "PlotCondition";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Check", Direction.In),
				new ConnectionDesc("OnTrue", Direction.Out),
				new ConnectionDesc("OnFalse", Direction.Out)
			};
        }

        public PlotConditionEntity(FrostySdk.Ebx.PlotConditionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIAnalogInputSelectionEntityData))]
	public class UIAnalogInputSelectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIAnalogInputSelectionEntityData>
	{
		public new FrostySdk.Ebx.UIAnalogInputSelectionEntityData Data => data as FrostySdk.Ebx.UIAnalogInputSelectionEntityData;
		public override string DisplayName => "UIAnalogInputSelection";
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Enable", Direction.In),
				new ConnectionDesc("Disable", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("DeadZonePercent", Direction.In),
				new ConnectionDesc("StickXValue", Direction.In),
				new ConnectionDesc("StickYValue", Direction.In)
			};
		}

		public UIAnalogInputSelectionEntity(FrostySdk.Ebx.UIAnalogInputSelectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


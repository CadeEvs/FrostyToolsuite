using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotIncrementIntegerByValueEntityData))]
	public class PlotIncrementIntegerByValueEntity : PlotIncrementIntegerBaseEntity, IEntityData<FrostySdk.Ebx.PlotIncrementIntegerByValueEntityData>
	{
		public new FrostySdk.Ebx.PlotIncrementIntegerByValueEntityData Data => data as FrostySdk.Ebx.PlotIncrementIntegerByValueEntityData;
		public override string DisplayName => "PlotIncrementIntegerByValue";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Increment", Direction.In),
				new ConnectionDesc("OnIncrement", Direction.Out)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Value", Direction.In)
			};
		}

		public PlotIncrementIntegerByValueEntity(FrostySdk.Ebx.PlotIncrementIntegerByValueEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


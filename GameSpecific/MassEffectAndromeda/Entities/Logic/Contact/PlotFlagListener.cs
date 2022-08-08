using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotFlagListenerData))]
	public class PlotFlagListener : LogicEntity, IEntityData<FrostySdk.Ebx.PlotFlagListenerData>
	{
		public new FrostySdk.Ebx.PlotFlagListenerData Data => data as FrostySdk.Ebx.PlotFlagListenerData;
		public override string DisplayName => "PlotFlagListener";

		public PlotFlagListener(FrostySdk.Ebx.PlotFlagListenerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


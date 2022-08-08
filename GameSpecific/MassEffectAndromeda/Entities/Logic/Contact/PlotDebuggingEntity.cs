using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlotDebuggingEntityData))]
	public class PlotDebuggingEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlotDebuggingEntityData>
	{
		public new FrostySdk.Ebx.PlotDebuggingEntityData Data => data as FrostySdk.Ebx.PlotDebuggingEntityData;
		public override string DisplayName => "PlotDebugging";

		public PlotDebuggingEntity(FrostySdk.Ebx.PlotDebuggingEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


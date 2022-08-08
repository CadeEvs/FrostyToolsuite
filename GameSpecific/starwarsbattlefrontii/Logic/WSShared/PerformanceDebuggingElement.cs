using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PerformanceDebuggingElementData))]
	public class PerformanceDebuggingElement : WSUISoloBatchableElement, IEntityData<FrostySdk.Ebx.PerformanceDebuggingElementData>
	{
		public new FrostySdk.Ebx.PerformanceDebuggingElementData Data => data as FrostySdk.Ebx.PerformanceDebuggingElementData;
		public override string DisplayName => "PerformanceDebuggingElement";

		public PerformanceDebuggingElement(FrostySdk.Ebx.PerformanceDebuggingElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


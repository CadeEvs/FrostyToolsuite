using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CircleElementData))]
	public class CircleElement : WSUISoloBatchableElement, IEntityData<FrostySdk.Ebx.CircleElementData>
	{
		public new FrostySdk.Ebx.CircleElementData Data => data as FrostySdk.Ebx.CircleElementData;
		public override string DisplayName => "CircleElement";

		public CircleElement(FrostySdk.Ebx.CircleElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


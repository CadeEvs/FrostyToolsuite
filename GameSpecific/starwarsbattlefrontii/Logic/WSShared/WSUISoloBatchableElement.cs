using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSUISoloBatchableElementData))]
	public class WSUISoloBatchableElement : WSUIBatchableElement, IEntityData<FrostySdk.Ebx.WSUISoloBatchableElementData>
	{
		public new FrostySdk.Ebx.WSUISoloBatchableElementData Data => data as FrostySdk.Ebx.WSUISoloBatchableElementData;
		public override string DisplayName => "WSUISoloBatchableElement";

		public WSUISoloBatchableElement(FrostySdk.Ebx.WSUISoloBatchableElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


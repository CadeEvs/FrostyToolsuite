using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSUIBatchableElementData))]
	public class WSUIBatchableElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.WSUIBatchableElementData>
	{
		public new FrostySdk.Ebx.WSUIBatchableElementData Data => data as FrostySdk.Ebx.WSUIBatchableElementData;
		public override string DisplayName => "WSUIBatchableElement";

		public WSUIBatchableElement(FrostySdk.Ebx.WSUIBatchableElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


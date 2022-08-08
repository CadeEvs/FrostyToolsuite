using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSUIElementFillEntityData))]
	public class WSUIElementFillEntity : WSUIBatchableElement, IEntityData<FrostySdk.Ebx.WSUIElementFillEntityData>
	{
		public new FrostySdk.Ebx.WSUIElementFillEntityData Data => data as FrostySdk.Ebx.WSUIElementFillEntityData;
		public override string DisplayName => "WSUIElementFill";

		public WSUIElementFillEntity(FrostySdk.Ebx.WSUIElementFillEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


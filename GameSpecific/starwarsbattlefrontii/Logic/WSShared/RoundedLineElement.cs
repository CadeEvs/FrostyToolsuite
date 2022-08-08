using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RoundedLineElementData))]
	public class RoundedLineElement : WSUIBatchableElement, IEntityData<FrostySdk.Ebx.RoundedLineElementData>
	{
		public new FrostySdk.Ebx.RoundedLineElementData Data => data as FrostySdk.Ebx.RoundedLineElementData;
		public override string DisplayName => "RoundedLineElement";

		public RoundedLineElement(FrostySdk.Ebx.RoundedLineElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


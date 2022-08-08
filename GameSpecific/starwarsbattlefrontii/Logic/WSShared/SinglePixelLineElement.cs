using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SinglePixelLineElementData))]
	public class SinglePixelLineElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.SinglePixelLineElementData>
	{
		public new FrostySdk.Ebx.SinglePixelLineElementData Data => data as FrostySdk.Ebx.SinglePixelLineElementData;
		public override string DisplayName => "SinglePixelLineElement";

		public SinglePixelLineElement(FrostySdk.Ebx.SinglePixelLineElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


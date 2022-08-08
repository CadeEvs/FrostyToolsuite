using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SvgElementData))]
	public class SvgElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.SvgElementData>
	{
		public new FrostySdk.Ebx.SvgElementData Data => data as FrostySdk.Ebx.SvgElementData;
		public override string DisplayName => "SvgElement";

		public SvgElement(FrostySdk.Ebx.SvgElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RegularPolygonElementData))]
	public class RegularPolygonElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.RegularPolygonElementData>
	{
		public new FrostySdk.Ebx.RegularPolygonElementData Data => data as FrostySdk.Ebx.RegularPolygonElementData;
		public override string DisplayName => "RegularPolygonElement";

		public RegularPolygonElement(FrostySdk.Ebx.RegularPolygonElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


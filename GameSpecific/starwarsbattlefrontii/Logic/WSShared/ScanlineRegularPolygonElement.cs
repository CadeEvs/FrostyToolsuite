using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScanlineRegularPolygonElementData))]
	public class ScanlineRegularPolygonElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.ScanlineRegularPolygonElementData>
	{
		public new FrostySdk.Ebx.ScanlineRegularPolygonElementData Data => data as FrostySdk.Ebx.ScanlineRegularPolygonElementData;
		public override string DisplayName => "ScanlineRegularPolygonElement";

		public ScanlineRegularPolygonElement(FrostySdk.Ebx.ScanlineRegularPolygonElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


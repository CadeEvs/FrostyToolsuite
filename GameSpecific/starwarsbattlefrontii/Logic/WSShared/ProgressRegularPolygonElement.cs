using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ProgressRegularPolygonElementData))]
	public class ProgressRegularPolygonElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.ProgressRegularPolygonElementData>
	{
		public new FrostySdk.Ebx.ProgressRegularPolygonElementData Data => data as FrostySdk.Ebx.ProgressRegularPolygonElementData;
		public override string DisplayName => "ProgressRegularPolygonElement";

		public ProgressRegularPolygonElement(FrostySdk.Ebx.ProgressRegularPolygonElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


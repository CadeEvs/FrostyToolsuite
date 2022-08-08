using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GridElementData))]
	public class GridElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.GridElementData>
	{
		public new FrostySdk.Ebx.GridElementData Data => data as FrostySdk.Ebx.GridElementData;
		public override string DisplayName => "GridElement";

		public GridElement(FrostySdk.Ebx.GridElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RectangleElementData))]
	public class RectangleElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.RectangleElementData>
	{
		public new FrostySdk.Ebx.RectangleElementData Data => data as FrostySdk.Ebx.RectangleElementData;
		public override string DisplayName => "RectangleElement";

		public RectangleElement(FrostySdk.Ebx.RectangleElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


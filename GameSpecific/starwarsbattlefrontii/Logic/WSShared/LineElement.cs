using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LineElementData))]
	public class LineElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.LineElementData>
	{
		public new FrostySdk.Ebx.LineElementData Data => data as FrostySdk.Ebx.LineElementData;
		public override string DisplayName => "LineElement";

		public LineElement(FrostySdk.Ebx.LineElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


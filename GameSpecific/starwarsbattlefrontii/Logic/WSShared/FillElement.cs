using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FillElementData))]
	public class FillElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.FillElementData>
	{
		public new FrostySdk.Ebx.FillElementData Data => data as FrostySdk.Ebx.FillElementData;
		public override string DisplayName => "FillElement";

		public FillElement(FrostySdk.Ebx.FillElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


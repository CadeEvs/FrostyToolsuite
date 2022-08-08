using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputElementData))]
	public class InputElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.InputElementData>
	{
		public new FrostySdk.Ebx.InputElementData Data => data as FrostySdk.Ebx.InputElementData;
		public override string DisplayName => "InputElement";

		public InputElement(FrostySdk.Ebx.InputElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


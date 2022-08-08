using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputConceptIconElementData))]
	public class InputConceptIconElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.InputConceptIconElementData>
	{
		public new FrostySdk.Ebx.InputConceptIconElementData Data => data as FrostySdk.Ebx.InputConceptIconElementData;
		public override string DisplayName => "InputConceptIconElement";

		public InputConceptIconElement(FrostySdk.Ebx.InputConceptIconElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


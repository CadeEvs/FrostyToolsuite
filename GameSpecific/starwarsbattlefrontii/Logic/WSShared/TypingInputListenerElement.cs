using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TypingInputListenerElementData))]
	public class TypingInputListenerElement : WSUIElementEntity, IEntityData<FrostySdk.Ebx.TypingInputListenerElementData>
	{
		public new FrostySdk.Ebx.TypingInputListenerElementData Data => data as FrostySdk.Ebx.TypingInputListenerElementData;
		public override string DisplayName => "TypingInputListenerElement";

		public TypingInputListenerElement(FrostySdk.Ebx.TypingInputListenerElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


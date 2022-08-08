using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DiceUITypingInputListenerElementData))]
	public class DiceUITypingInputListenerElement : UIElementEntity, IEntityData<FrostySdk.Ebx.DiceUITypingInputListenerElementData>
	{
		public new FrostySdk.Ebx.DiceUITypingInputListenerElementData Data => data as FrostySdk.Ebx.DiceUITypingInputListenerElementData;
		public override string DisplayName => "DiceUITypingInputListenerElement";

		public DiceUITypingInputListenerElement(FrostySdk.Ebx.DiceUITypingInputListenerElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


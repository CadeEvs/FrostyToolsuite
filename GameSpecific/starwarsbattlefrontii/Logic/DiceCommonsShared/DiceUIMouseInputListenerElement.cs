using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DiceUIMouseInputListenerElementData))]
	public class DiceUIMouseInputListenerElement : UIElementEntity, IEntityData<FrostySdk.Ebx.DiceUIMouseInputListenerElementData>
	{
		public new FrostySdk.Ebx.DiceUIMouseInputListenerElementData Data => data as FrostySdk.Ebx.DiceUIMouseInputListenerElementData;
		public override string DisplayName => "DiceUIMouseInputListenerElement";

		public DiceUIMouseInputListenerElement(FrostySdk.Ebx.DiceUIMouseInputListenerElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


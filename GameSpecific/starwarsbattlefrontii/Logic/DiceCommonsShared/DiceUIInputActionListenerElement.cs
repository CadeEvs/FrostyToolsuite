using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DiceUIInputActionListenerElementData))]
	public class DiceUIInputActionListenerElement : UIElementEntity, IEntityData<FrostySdk.Ebx.DiceUIInputActionListenerElementData>
	{
		public new FrostySdk.Ebx.DiceUIInputActionListenerElementData Data => data as FrostySdk.Ebx.DiceUIInputActionListenerElementData;
		public override string DisplayName => "DiceUIInputActionListenerElement";

		public DiceUIInputActionListenerElement(FrostySdk.Ebx.DiceUIInputActionListenerElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


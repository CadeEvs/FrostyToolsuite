using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DiceUIAnalogPadInputListenerElementData))]
	public class DiceUIAnalogPadInputListenerElement : UIElementEntity, IEntityData<FrostySdk.Ebx.DiceUIAnalogPadInputListenerElementData>
	{
		public new FrostySdk.Ebx.DiceUIAnalogPadInputListenerElementData Data => data as FrostySdk.Ebx.DiceUIAnalogPadInputListenerElementData;
		public override string DisplayName => "DiceUIAnalogPadInputListenerElement";

		public DiceUIAnalogPadInputListenerElement(FrostySdk.Ebx.DiceUIAnalogPadInputListenerElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


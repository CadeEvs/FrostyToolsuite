using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DiceUIAnalogStickInputListenerElementData))]
	public class DiceUIAnalogStickInputListenerElement : UIElementEntity, IEntityData<FrostySdk.Ebx.DiceUIAnalogStickInputListenerElementData>
	{
		public new FrostySdk.Ebx.DiceUIAnalogStickInputListenerElementData Data => data as FrostySdk.Ebx.DiceUIAnalogStickInputListenerElementData;
		public override string DisplayName => "DiceUIAnalogStickInputListenerElement";

		public DiceUIAnalogStickInputListenerElement(FrostySdk.Ebx.DiceUIAnalogStickInputListenerElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


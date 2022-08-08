using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DiceUIInputBlockerElementData))]
	public class DiceUIInputBlockerElement : UIElementEntity, IEntityData<FrostySdk.Ebx.DiceUIInputBlockerElementData>
	{
		public new FrostySdk.Ebx.DiceUIInputBlockerElementData Data => data as FrostySdk.Ebx.DiceUIInputBlockerElementData;
		public override string DisplayName => "DiceUIInputBlockerElement";

		public DiceUIInputBlockerElement(FrostySdk.Ebx.DiceUIInputBlockerElementData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


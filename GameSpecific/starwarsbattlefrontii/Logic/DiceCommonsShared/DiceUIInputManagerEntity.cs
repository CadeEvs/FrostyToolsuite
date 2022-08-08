using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DiceUIInputManagerEntityData))]
	public class DiceUIInputManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DiceUIInputManagerEntityData>
	{
		public new FrostySdk.Ebx.DiceUIInputManagerEntityData Data => data as FrostySdk.Ebx.DiceUIInputManagerEntityData;
		public override string DisplayName => "DiceUIInputManager";

		public DiceUIInputManagerEntity(FrostySdk.Ebx.DiceUIInputManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DiceVFXEntityBaseData))]
	public class DiceVFXEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.DiceVFXEntityBaseData>
	{
		public new FrostySdk.Ebx.DiceVFXEntityBaseData Data => data as FrostySdk.Ebx.DiceVFXEntityBaseData;
		public override string DisplayName => "DiceVFXEntityBase";

		public DiceVFXEntityBase(FrostySdk.Ebx.DiceVFXEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


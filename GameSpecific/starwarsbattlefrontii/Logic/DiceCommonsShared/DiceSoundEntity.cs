using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DiceSoundEntityData))]
	public class DiceSoundEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DiceSoundEntityData>
	{
		public new FrostySdk.Ebx.DiceSoundEntityData Data => data as FrostySdk.Ebx.DiceSoundEntityData;
		public override string DisplayName => "DiceSound";

		public DiceSoundEntity(FrostySdk.Ebx.DiceSoundEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


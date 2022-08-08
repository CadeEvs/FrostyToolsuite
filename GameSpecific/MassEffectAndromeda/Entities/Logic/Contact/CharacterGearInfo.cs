using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterGearInfoData))]
	public class CharacterGearInfo : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterGearInfoData>
	{
		public new FrostySdk.Ebx.CharacterGearInfoData Data => data as FrostySdk.Ebx.CharacterGearInfoData;
		public override string DisplayName => "CharacterGearInfo";

		public CharacterGearInfo(FrostySdk.Ebx.CharacterGearInfoData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


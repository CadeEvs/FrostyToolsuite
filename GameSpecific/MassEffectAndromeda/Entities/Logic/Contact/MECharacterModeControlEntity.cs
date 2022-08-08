using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECharacterModeControlEntityData))]
	public class MECharacterModeControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MECharacterModeControlEntityData>
	{
		public new FrostySdk.Ebx.MECharacterModeControlEntityData Data => data as FrostySdk.Ebx.MECharacterModeControlEntityData;
		public override string DisplayName => "MECharacterModeControl";

		public MECharacterModeControlEntity(FrostySdk.Ebx.MECharacterModeControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


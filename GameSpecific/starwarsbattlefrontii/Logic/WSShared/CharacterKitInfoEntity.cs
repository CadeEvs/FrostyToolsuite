using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterKitInfoEntityData))]
	public class CharacterKitInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterKitInfoEntityData>
	{
		public new FrostySdk.Ebx.CharacterKitInfoEntityData Data => data as FrostySdk.Ebx.CharacterKitInfoEntityData;
		public override string DisplayName => "CharacterKitInfo";

		public CharacterKitInfoEntity(FrostySdk.Ebx.CharacterKitInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


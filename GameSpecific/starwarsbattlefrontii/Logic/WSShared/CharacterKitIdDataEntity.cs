using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterKitIdDataEntityData))]
	public class CharacterKitIdDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterKitIdDataEntityData>
	{
		public new FrostySdk.Ebx.CharacterKitIdDataEntityData Data => data as FrostySdk.Ebx.CharacterKitIdDataEntityData;
		public override string DisplayName => "CharacterKitIdData";

		public CharacterKitIdDataEntity(FrostySdk.Ebx.CharacterKitIdDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterPathTestEntityData))]
	public class CharacterPathTestEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterPathTestEntityData>
	{
		public new FrostySdk.Ebx.CharacterPathTestEntityData Data => data as FrostySdk.Ebx.CharacterPathTestEntityData;
		public override string DisplayName => "CharacterPathTest";

		public CharacterPathTestEntity(FrostySdk.Ebx.CharacterPathTestEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


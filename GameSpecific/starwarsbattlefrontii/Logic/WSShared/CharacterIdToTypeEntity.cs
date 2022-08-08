using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterIdToTypeEntityData))]
	public class CharacterIdToTypeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CharacterIdToTypeEntityData>
	{
		public new FrostySdk.Ebx.CharacterIdToTypeEntityData Data => data as FrostySdk.Ebx.CharacterIdToTypeEntityData;
		public override string DisplayName => "CharacterIdToType";

		public CharacterIdToTypeEntity(FrostySdk.Ebx.CharacterIdToTypeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


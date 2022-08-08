using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterEntityData))]
	public class CharacterEntity : ControllableEntity, IEntityData<FrostySdk.Ebx.CharacterEntityData>
	{
		public new FrostySdk.Ebx.CharacterEntityData Data => data as FrostySdk.Ebx.CharacterEntityData;

		public CharacterEntity(FrostySdk.Ebx.CharacterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


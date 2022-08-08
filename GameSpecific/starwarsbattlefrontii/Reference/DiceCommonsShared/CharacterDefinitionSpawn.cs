using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterDefinitionSpawnData))]
	public class CharacterDefinitionSpawn : CharacterSpawnReferenceObject, IEntityData<FrostySdk.Ebx.CharacterDefinitionSpawnData>
	{
		public new FrostySdk.Ebx.CharacterDefinitionSpawnData Data => data as FrostySdk.Ebx.CharacterDefinitionSpawnData;

		public CharacterDefinitionSpawn(FrostySdk.Ebx.CharacterDefinitionSpawnData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


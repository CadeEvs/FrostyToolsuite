using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterSpawnReferenceObjectData))]
	public class CharacterSpawnReferenceObject : SpawnReferenceObject, IEntityData<FrostySdk.Ebx.CharacterSpawnReferenceObjectData>
	{
		public new FrostySdk.Ebx.CharacterSpawnReferenceObjectData Data => data as FrostySdk.Ebx.CharacterSpawnReferenceObjectData;

		public CharacterSpawnReferenceObject(FrostySdk.Ebx.CharacterSpawnReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


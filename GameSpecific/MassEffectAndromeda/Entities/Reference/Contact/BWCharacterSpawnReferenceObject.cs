using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWCharacterSpawnReferenceObjectData))]
	public class BWCharacterSpawnReferenceObject : CharacterSpawnReferenceObject, IEntityData<FrostySdk.Ebx.BWCharacterSpawnReferenceObjectData>
	{
		public new FrostySdk.Ebx.BWCharacterSpawnReferenceObjectData Data => data as FrostySdk.Ebx.BWCharacterSpawnReferenceObjectData;

		public BWCharacterSpawnReferenceObject(FrostySdk.Ebx.BWCharacterSpawnReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


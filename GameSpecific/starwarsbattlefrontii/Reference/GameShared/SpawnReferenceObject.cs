using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpawnReferenceObjectData))]
	public class SpawnReferenceObject : GameplaySpawnReferenceObject, IEntityData<FrostySdk.Ebx.SpawnReferenceObjectData>
	{
		public new FrostySdk.Ebx.SpawnReferenceObjectData Data => data as FrostySdk.Ebx.SpawnReferenceObjectData;

		public SpawnReferenceObject(FrostySdk.Ebx.SpawnReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


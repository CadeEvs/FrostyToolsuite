using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameplaySpawnReferenceObjectData))]
	public class GameplaySpawnReferenceObject : SpatialReferenceObject, IEntityData<FrostySdk.Ebx.GameplaySpawnReferenceObjectData>
	{
		public new FrostySdk.Ebx.GameplaySpawnReferenceObjectData Data => data as FrostySdk.Ebx.GameplaySpawnReferenceObjectData;

		public GameplaySpawnReferenceObject(FrostySdk.Ebx.GameplaySpawnReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


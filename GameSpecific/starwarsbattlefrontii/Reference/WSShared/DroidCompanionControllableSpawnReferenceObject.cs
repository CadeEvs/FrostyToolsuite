using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidCompanionControllableSpawnReferenceObjectData))]
	public class DroidCompanionControllableSpawnReferenceObject : SpawnReferenceObject, IEntityData<FrostySdk.Ebx.DroidCompanionControllableSpawnReferenceObjectData>
	{
		public new FrostySdk.Ebx.DroidCompanionControllableSpawnReferenceObjectData Data => data as FrostySdk.Ebx.DroidCompanionControllableSpawnReferenceObjectData;

		public DroidCompanionControllableSpawnReferenceObject(FrostySdk.Ebx.DroidCompanionControllableSpawnReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


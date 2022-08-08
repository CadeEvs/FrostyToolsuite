using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SoldierSpawnReferenceObjectData))]
	public class SoldierSpawnReferenceObject : CharacterSpawnReferenceObject, IEntityData<FrostySdk.Ebx.SoldierSpawnReferenceObjectData>
	{
		public new FrostySdk.Ebx.SoldierSpawnReferenceObjectData Data => data as FrostySdk.Ebx.SoldierSpawnReferenceObjectData;

		public SoldierSpawnReferenceObject(FrostySdk.Ebx.SoldierSpawnReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


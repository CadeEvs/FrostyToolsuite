using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RemnantSpawnReferenceObjectData))]
	public class RemnantSpawnReferenceObject : CharacterSpawnReferenceObject, IEntityData<FrostySdk.Ebx.RemnantSpawnReferenceObjectData>
	{
		public new FrostySdk.Ebx.RemnantSpawnReferenceObjectData Data => data as FrostySdk.Ebx.RemnantSpawnReferenceObjectData;

		public RemnantSpawnReferenceObject(FrostySdk.Ebx.RemnantSpawnReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


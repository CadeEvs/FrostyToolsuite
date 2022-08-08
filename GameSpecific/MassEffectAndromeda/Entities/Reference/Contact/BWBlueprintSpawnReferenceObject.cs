using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWBlueprintSpawnReferenceObjectData))]
	public class BWBlueprintSpawnReferenceObject : BlueprintSpawnReferenceObject, IEntityData<FrostySdk.Ebx.BWBlueprintSpawnReferenceObjectData>
	{
		public new FrostySdk.Ebx.BWBlueprintSpawnReferenceObjectData Data => data as FrostySdk.Ebx.BWBlueprintSpawnReferenceObjectData;

		public BWBlueprintSpawnReferenceObject(FrostySdk.Ebx.BWBlueprintSpawnReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


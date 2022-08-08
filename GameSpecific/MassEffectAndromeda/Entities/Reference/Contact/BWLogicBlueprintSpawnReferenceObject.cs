using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWLogicBlueprintSpawnReferenceObjectData))]
	public class BWLogicBlueprintSpawnReferenceObject : BWBlueprintSpawnReferenceObject, IEntityData<FrostySdk.Ebx.BWLogicBlueprintSpawnReferenceObjectData>
	{
		public new FrostySdk.Ebx.BWLogicBlueprintSpawnReferenceObjectData Data => data as FrostySdk.Ebx.BWLogicBlueprintSpawnReferenceObjectData;

		public BWLogicBlueprintSpawnReferenceObject(FrostySdk.Ebx.BWLogicBlueprintSpawnReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


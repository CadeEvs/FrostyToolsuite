using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEStorytellerSpawnReferenceObjectData))]
	public class MEStorytellerSpawnReferenceObject : BWBlueprintSpawnReferenceObject, IEntityData<FrostySdk.Ebx.MEStorytellerSpawnReferenceObjectData>
	{
		public new FrostySdk.Ebx.MEStorytellerSpawnReferenceObjectData Data => data as FrostySdk.Ebx.MEStorytellerSpawnReferenceObjectData;

		public MEStorytellerSpawnReferenceObject(FrostySdk.Ebx.MEStorytellerSpawnReferenceObjectData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


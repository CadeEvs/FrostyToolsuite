using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WorldInteractionEntityData))]
	public class WorldInteractionEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.WorldInteractionEntityData>
	{
		public new FrostySdk.Ebx.WorldInteractionEntityData Data => data as FrostySdk.Ebx.WorldInteractionEntityData;

		public WorldInteractionEntity(FrostySdk.Ebx.WorldInteractionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


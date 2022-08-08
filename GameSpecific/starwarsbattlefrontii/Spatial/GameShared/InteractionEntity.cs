using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InteractionEntityData))]
	public class InteractionEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.InteractionEntityData>
	{
		public new FrostySdk.Ebx.InteractionEntityData Data => data as FrostySdk.Ebx.InteractionEntityData;

		public InteractionEntity(FrostySdk.Ebx.InteractionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


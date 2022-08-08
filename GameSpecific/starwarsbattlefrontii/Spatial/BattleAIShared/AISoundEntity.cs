using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AISoundEntityData))]
	public class AISoundEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.AISoundEntityData>
	{
		public new FrostySdk.Ebx.AISoundEntityData Data => data as FrostySdk.Ebx.AISoundEntityData;

		public AISoundEntity(FrostySdk.Ebx.AISoundEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


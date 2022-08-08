using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ExcavationEntityData))]
	public class ExcavationEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.ExcavationEntityData>
	{
		public new FrostySdk.Ebx.ExcavationEntityData Data => data as FrostySdk.Ebx.ExcavationEntityData;

		public ExcavationEntity(FrostySdk.Ebx.ExcavationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


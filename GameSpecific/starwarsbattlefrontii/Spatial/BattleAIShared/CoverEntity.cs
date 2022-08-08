using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoverEntityData))]
	public class CoverEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.CoverEntityData>
	{
		public new FrostySdk.Ebx.CoverEntityData Data => data as FrostySdk.Ebx.CoverEntityData;

		public CoverEntity(FrostySdk.Ebx.CoverEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


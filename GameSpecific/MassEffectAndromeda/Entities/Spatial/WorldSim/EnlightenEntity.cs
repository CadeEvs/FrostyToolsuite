using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.EnlightenEntityData))]
	public class EnlightenEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.EnlightenEntityData>
	{
		public new FrostySdk.Ebx.EnlightenEntityData Data => data as FrostySdk.Ebx.EnlightenEntityData;

		public EnlightenEntity(FrostySdk.Ebx.EnlightenEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


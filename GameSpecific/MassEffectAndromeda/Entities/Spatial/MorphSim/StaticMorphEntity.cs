using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StaticMorphEntityData))]
	public class StaticMorphEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.StaticMorphEntityData>
	{
		public new FrostySdk.Ebx.StaticMorphEntityData Data => data as FrostySdk.Ebx.StaticMorphEntityData;

		public StaticMorphEntity(FrostySdk.Ebx.StaticMorphEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


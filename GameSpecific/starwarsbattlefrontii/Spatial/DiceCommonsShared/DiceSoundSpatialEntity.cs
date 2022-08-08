using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DiceSoundSpatialEntityData))]
	public class DiceSoundSpatialEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.DiceSoundSpatialEntityData>
	{
		public new FrostySdk.Ebx.DiceSoundSpatialEntityData Data => data as FrostySdk.Ebx.DiceSoundSpatialEntityData;

		public DiceSoundSpatialEntity(FrostySdk.Ebx.DiceSoundSpatialEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


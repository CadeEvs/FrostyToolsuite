using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LevelTransitionLocationMarkerEntityData))]
	public class LevelTransitionLocationMarkerEntity : LocationMarkerEntity, IEntityData<FrostySdk.Ebx.LevelTransitionLocationMarkerEntityData>
	{
		public new FrostySdk.Ebx.LevelTransitionLocationMarkerEntityData Data => data as FrostySdk.Ebx.LevelTransitionLocationMarkerEntityData;

		public LevelTransitionLocationMarkerEntity(FrostySdk.Ebx.LevelTransitionLocationMarkerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


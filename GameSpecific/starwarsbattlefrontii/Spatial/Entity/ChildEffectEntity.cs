using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChildEffectEntityData))]
	public class ChildEffectEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.ChildEffectEntityData>
	{
		public new FrostySdk.Ebx.ChildEffectEntityData Data => data as FrostySdk.Ebx.ChildEffectEntityData;

		public ChildEffectEntity(FrostySdk.Ebx.ChildEffectEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


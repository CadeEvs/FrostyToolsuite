using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LadderEntityData))]
	public class LadderEntity : StaticModelEntity, IEntityData<FrostySdk.Ebx.LadderEntityData>
	{
		public new FrostySdk.Ebx.LadderEntityData Data => data as FrostySdk.Ebx.LadderEntityData;

		public LadderEntity(FrostySdk.Ebx.LadderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


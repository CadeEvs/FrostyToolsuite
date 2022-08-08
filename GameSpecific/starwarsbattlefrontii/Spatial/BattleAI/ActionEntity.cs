using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ActionEntityData))]
	public class ActionEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.ActionEntityData>
	{
		public new FrostySdk.Ebx.ActionEntityData Data => data as FrostySdk.Ebx.ActionEntityData;

		public ActionEntity(FrostySdk.Ebx.ActionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AwarenessInfoEntityData))]
	public class AwarenessInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AwarenessInfoEntityData>
	{
		public new FrostySdk.Ebx.AwarenessInfoEntityData Data => data as FrostySdk.Ebx.AwarenessInfoEntityData;
		public override string DisplayName => "AwarenessInfo";

		public AwarenessInfoEntity(FrostySdk.Ebx.AwarenessInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


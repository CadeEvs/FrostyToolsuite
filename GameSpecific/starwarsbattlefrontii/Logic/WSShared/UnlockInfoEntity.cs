using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UnlockInfoEntityData))]
	public class UnlockInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UnlockInfoEntityData>
	{
		public new FrostySdk.Ebx.UnlockInfoEntityData Data => data as FrostySdk.Ebx.UnlockInfoEntityData;
		public override string DisplayName => "UnlockInfo";

		public UnlockInfoEntity(FrostySdk.Ebx.UnlockInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


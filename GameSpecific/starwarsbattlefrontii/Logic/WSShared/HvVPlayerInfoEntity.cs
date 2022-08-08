using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HvVPlayerInfoEntityData))]
	public class HvVPlayerInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HvVPlayerInfoEntityData>
	{
		public new FrostySdk.Ebx.HvVPlayerInfoEntityData Data => data as FrostySdk.Ebx.HvVPlayerInfoEntityData;
		public override string DisplayName => "HvVPlayerInfo";

		public HvVPlayerInfoEntity(FrostySdk.Ebx.HvVPlayerInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoopSpawnHelperEntityData))]
	public class CoopSpawnHelperEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CoopSpawnHelperEntityData>
	{
		public new FrostySdk.Ebx.CoopSpawnHelperEntityData Data => data as FrostySdk.Ebx.CoopSpawnHelperEntityData;
		public override string DisplayName => "CoopSpawnHelper";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CoopSpawnHelperEntity(FrostySdk.Ebx.CoopSpawnHelperEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


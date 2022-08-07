using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatCacheEntityData))]
	public class FloatCacheEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FloatCacheEntityData>
	{
		public new FrostySdk.Ebx.FloatCacheEntityData Data => data as FrostySdk.Ebx.FloatCacheEntityData;
		public override string DisplayName => "FloatCache";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public FloatCacheEntity(FrostySdk.Ebx.FloatCacheEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


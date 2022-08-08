using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyCacheEntityData))]
	public class PropertyCacheEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PropertyCacheEntityData>
	{
		public new FrostySdk.Ebx.PropertyCacheEntityData Data => data as FrostySdk.Ebx.PropertyCacheEntityData;
		public override string DisplayName => "PropertyCache";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PropertyCacheEntity(FrostySdk.Ebx.PropertyCacheEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


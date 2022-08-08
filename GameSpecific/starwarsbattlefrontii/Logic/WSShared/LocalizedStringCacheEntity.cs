using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalizedStringCacheEntityData))]
	public class LocalizedStringCacheEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocalizedStringCacheEntityData>
	{
		public new FrostySdk.Ebx.LocalizedStringCacheEntityData Data => data as FrostySdk.Ebx.LocalizedStringCacheEntityData;
		public override string DisplayName => "LocalizedStringCache";

		public LocalizedStringCacheEntity(FrostySdk.Ebx.LocalizedStringCacheEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


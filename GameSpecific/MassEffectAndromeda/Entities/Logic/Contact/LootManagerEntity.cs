using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LootManagerEntityData))]
	public class LootManagerEntity : SingletonEntity, IEntityData<FrostySdk.Ebx.LootManagerEntityData>
	{
		public new FrostySdk.Ebx.LootManagerEntityData Data => data as FrostySdk.Ebx.LootManagerEntityData;
		public override string DisplayName => "LootManager";

		public LootManagerEntity(FrostySdk.Ebx.LootManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


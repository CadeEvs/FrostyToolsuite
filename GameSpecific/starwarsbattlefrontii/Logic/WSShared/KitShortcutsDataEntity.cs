using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.KitShortcutsDataEntityData))]
	public class KitShortcutsDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.KitShortcutsDataEntityData>
	{
		public new FrostySdk.Ebx.KitShortcutsDataEntityData Data => data as FrostySdk.Ebx.KitShortcutsDataEntityData;
		public override string DisplayName => "KitShortcutsData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public KitShortcutsDataEntity(FrostySdk.Ebx.KitShortcutsDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


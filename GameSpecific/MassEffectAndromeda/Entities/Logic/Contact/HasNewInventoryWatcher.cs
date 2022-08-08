using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HasNewInventoryWatcherData))]
	public class HasNewInventoryWatcher : LogicEntity, IEntityData<FrostySdk.Ebx.HasNewInventoryWatcherData>
	{
		public new FrostySdk.Ebx.HasNewInventoryWatcherData Data => data as FrostySdk.Ebx.HasNewInventoryWatcherData;
		public override string DisplayName => "HasNewInventoryWatcher";

		public HasNewInventoryWatcher(FrostySdk.Ebx.HasNewInventoryWatcherData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


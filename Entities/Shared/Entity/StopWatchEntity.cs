using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StopWatchEntityData))]
	public class StopWatchEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StopWatchEntityData>
	{
		public new FrostySdk.Ebx.StopWatchEntityData Data => data as FrostySdk.Ebx.StopWatchEntityData;
		public override string DisplayName => "StopWatch";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StopWatchEntity(FrostySdk.Ebx.StopWatchEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSLoadCheckpointTestData))]
	public class WSLoadCheckpointTest : LogicEntity, IEntityData<FrostySdk.Ebx.WSLoadCheckpointTestData>
	{
		public new FrostySdk.Ebx.WSLoadCheckpointTestData Data => data as FrostySdk.Ebx.WSLoadCheckpointTestData;
		public override string DisplayName => "WSLoadCheckpointTest";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSLoadCheckpointTest(FrostySdk.Ebx.WSLoadCheckpointTestData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


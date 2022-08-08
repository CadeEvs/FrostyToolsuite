using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSTestCaseEntityData))]
	public class WSTestCaseEntity : TestCaseEntity, IEntityData<FrostySdk.Ebx.WSTestCaseEntityData>
	{
		public new FrostySdk.Ebx.WSTestCaseEntityData Data => data as FrostySdk.Ebx.WSTestCaseEntityData;
		public override string DisplayName => "WSTestCase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSTestCaseEntity(FrostySdk.Ebx.WSTestCaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


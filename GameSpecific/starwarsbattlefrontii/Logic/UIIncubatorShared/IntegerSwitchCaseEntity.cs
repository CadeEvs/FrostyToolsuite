using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntegerSwitchCaseEntityData))]
	public class IntegerSwitchCaseEntity : BaseSwitchCaseEntity, IEntityData<FrostySdk.Ebx.IntegerSwitchCaseEntityData>
	{
		public new FrostySdk.Ebx.IntegerSwitchCaseEntityData Data => data as FrostySdk.Ebx.IntegerSwitchCaseEntityData;
		public override string DisplayName => "IntegerSwitchCase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public IntegerSwitchCaseEntity(FrostySdk.Ebx.IntegerSwitchCaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


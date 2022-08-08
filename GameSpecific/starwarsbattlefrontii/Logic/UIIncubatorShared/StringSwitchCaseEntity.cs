using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StringSwitchCaseEntityData))]
	public class StringSwitchCaseEntity : BaseSwitchCaseEntity, IEntityData<FrostySdk.Ebx.StringSwitchCaseEntityData>
	{
		public new FrostySdk.Ebx.StringSwitchCaseEntityData Data => data as FrostySdk.Ebx.StringSwitchCaseEntityData;
		public override string DisplayName => "StringSwitchCase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public StringSwitchCaseEntity(FrostySdk.Ebx.StringSwitchCaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


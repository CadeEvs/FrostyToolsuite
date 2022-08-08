using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BaseSwitchCaseEntityData))]
	public class BaseSwitchCaseEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BaseSwitchCaseEntityData>
	{
		public new FrostySdk.Ebx.BaseSwitchCaseEntityData Data => data as FrostySdk.Ebx.BaseSwitchCaseEntityData;
		public override string DisplayName => "BaseSwitchCase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BaseSwitchCaseEntity(FrostySdk.Ebx.BaseSwitchCaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


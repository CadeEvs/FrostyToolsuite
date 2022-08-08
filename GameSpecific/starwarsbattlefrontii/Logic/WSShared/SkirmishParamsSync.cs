using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SkirmishParamsSyncData))]
	public class SkirmishParamsSync : LogicEntity, IEntityData<FrostySdk.Ebx.SkirmishParamsSyncData>
	{
		public new FrostySdk.Ebx.SkirmishParamsSyncData Data => data as FrostySdk.Ebx.SkirmishParamsSyncData;
		public override string DisplayName => "SkirmishParamsSync";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SkirmishParamsSync(FrostySdk.Ebx.SkirmishParamsSyncData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


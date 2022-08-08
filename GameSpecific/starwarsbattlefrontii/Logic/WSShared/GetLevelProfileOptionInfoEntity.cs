using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetLevelProfileOptionInfoEntityData))]
	public class GetLevelProfileOptionInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GetLevelProfileOptionInfoEntityData>
	{
		public new FrostySdk.Ebx.GetLevelProfileOptionInfoEntityData Data => data as FrostySdk.Ebx.GetLevelProfileOptionInfoEntityData;
		public override string DisplayName => "GetLevelProfileOptionInfo";

		public GetLevelProfileOptionInfoEntity(FrostySdk.Ebx.GetLevelProfileOptionInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


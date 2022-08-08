using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetCodexDataEntityData))]
	public class GetCodexDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GetCodexDataEntityData>
	{
		public new FrostySdk.Ebx.GetCodexDataEntityData Data => data as FrostySdk.Ebx.GetCodexDataEntityData;
		public override string DisplayName => "GetCodexData";

		public GetCodexDataEntity(FrostySdk.Ebx.GetCodexDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


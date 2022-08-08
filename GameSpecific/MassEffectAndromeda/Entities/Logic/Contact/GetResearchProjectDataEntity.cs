using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetResearchProjectDataEntityData))]
	public class GetResearchProjectDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GetResearchProjectDataEntityData>
	{
		public new FrostySdk.Ebx.GetResearchProjectDataEntityData Data => data as FrostySdk.Ebx.GetResearchProjectDataEntityData;
		public override string DisplayName => "GetResearchProjectData";

		public GetResearchProjectDataEntity(FrostySdk.Ebx.GetResearchProjectDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


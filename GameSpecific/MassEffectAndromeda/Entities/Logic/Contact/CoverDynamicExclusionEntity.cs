using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CoverDynamicExclusionEntityData))]
	public class CoverDynamicExclusionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CoverDynamicExclusionEntityData>
	{
		public new FrostySdk.Ebx.CoverDynamicExclusionEntityData Data => data as FrostySdk.Ebx.CoverDynamicExclusionEntityData;
		public override string DisplayName => "CoverDynamicExclusion";

		public CoverDynamicExclusionEntity(FrostySdk.Ebx.CoverDynamicExclusionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


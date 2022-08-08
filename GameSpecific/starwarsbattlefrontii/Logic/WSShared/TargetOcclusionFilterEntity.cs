using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetOcclusionFilterEntityData))]
	public class TargetOcclusionFilterEntity : TargetFilterEntity, IEntityData<FrostySdk.Ebx.TargetOcclusionFilterEntityData>
	{
		public new FrostySdk.Ebx.TargetOcclusionFilterEntityData Data => data as FrostySdk.Ebx.TargetOcclusionFilterEntityData;
		public override string DisplayName => "TargetOcclusionFilter";

		public TargetOcclusionFilterEntity(FrostySdk.Ebx.TargetOcclusionFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


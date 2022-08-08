using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OcclusionConfigEntityData))]
	public class OcclusionConfigEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OcclusionConfigEntityData>
	{
		public new FrostySdk.Ebx.OcclusionConfigEntityData Data => data as FrostySdk.Ebx.OcclusionConfigEntityData;
		public override string DisplayName => "OcclusionConfig";

		public OcclusionConfigEntity(FrostySdk.Ebx.OcclusionConfigEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


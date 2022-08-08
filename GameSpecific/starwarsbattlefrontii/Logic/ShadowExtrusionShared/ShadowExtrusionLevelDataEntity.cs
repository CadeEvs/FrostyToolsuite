using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShadowExtrusionLevelDataEntityData))]
	public class ShadowExtrusionLevelDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ShadowExtrusionLevelDataEntityData>
	{
		public new FrostySdk.Ebx.ShadowExtrusionLevelDataEntityData Data => data as FrostySdk.Ebx.ShadowExtrusionLevelDataEntityData;
		public override string DisplayName => "ShadowExtrusionLevelData";

		public ShadowExtrusionLevelDataEntity(FrostySdk.Ebx.ShadowExtrusionLevelDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


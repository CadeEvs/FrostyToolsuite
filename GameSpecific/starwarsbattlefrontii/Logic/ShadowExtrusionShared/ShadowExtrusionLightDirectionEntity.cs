using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShadowExtrusionLightDirectionEntityData))]
	public class ShadowExtrusionLightDirectionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ShadowExtrusionLightDirectionEntityData>
	{
		public new FrostySdk.Ebx.ShadowExtrusionLightDirectionEntityData Data => data as FrostySdk.Ebx.ShadowExtrusionLightDirectionEntityData;
		public override string DisplayName => "ShadowExtrusionLightDirection";

		public ShadowExtrusionLightDirectionEntity(FrostySdk.Ebx.ShadowExtrusionLightDirectionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


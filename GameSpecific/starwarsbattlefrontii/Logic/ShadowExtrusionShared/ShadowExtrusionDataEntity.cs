using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ShadowExtrusionDataEntityData))]
	public class ShadowExtrusionDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ShadowExtrusionDataEntityData>
	{
		public new FrostySdk.Ebx.ShadowExtrusionDataEntityData Data => data as FrostySdk.Ebx.ShadowExtrusionDataEntityData;
		public override string DisplayName => "ShadowExtrusionData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ShadowExtrusionDataEntity(FrostySdk.Ebx.ShadowExtrusionDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


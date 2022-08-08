using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SplineSamplerEntityData))]
	public class SplineSamplerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SplineSamplerEntityData>
	{
		public new FrostySdk.Ebx.SplineSamplerEntityData Data => data as FrostySdk.Ebx.SplineSamplerEntityData;
		public override string DisplayName => "SplineSampler";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SplineSamplerEntity(FrostySdk.Ebx.SplineSamplerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


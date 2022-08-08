using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SpatialAnalyzerData))]
	public class SpatialAnalyzer : LogicEntity, IEntityData<FrostySdk.Ebx.SpatialAnalyzerData>
	{
		public new FrostySdk.Ebx.SpatialAnalyzerData Data => data as FrostySdk.Ebx.SpatialAnalyzerData;
		public override string DisplayName => "SpatialAnalyzer";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SpatialAnalyzer(FrostySdk.Ebx.SpatialAnalyzerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


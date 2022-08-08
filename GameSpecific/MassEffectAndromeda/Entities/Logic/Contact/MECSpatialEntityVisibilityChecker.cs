using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECSpatialEntityVisibilityCheckerData))]
	public class MECSpatialEntityVisibilityChecker : LogicEntity, IEntityData<FrostySdk.Ebx.MECSpatialEntityVisibilityCheckerData>
	{
		public new FrostySdk.Ebx.MECSpatialEntityVisibilityCheckerData Data => data as FrostySdk.Ebx.MECSpatialEntityVisibilityCheckerData;
		public override string DisplayName => "MECSpatialEntityVisibilityChecker";

		public MECSpatialEntityVisibilityChecker(FrostySdk.Ebx.MECSpatialEntityVisibilityCheckerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


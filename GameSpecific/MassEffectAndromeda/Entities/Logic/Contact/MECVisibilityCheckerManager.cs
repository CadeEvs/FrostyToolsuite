using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MECVisibilityCheckerManagerData))]
	public class MECVisibilityCheckerManager : LogicEntity, IEntityData<FrostySdk.Ebx.MECVisibilityCheckerManagerData>
	{
		public new FrostySdk.Ebx.MECVisibilityCheckerManagerData Data => data as FrostySdk.Ebx.MECVisibilityCheckerManagerData;
		public override string DisplayName => "MECVisibilityCheckerManager";

		public MECVisibilityCheckerManager(FrostySdk.Ebx.MECVisibilityCheckerManagerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


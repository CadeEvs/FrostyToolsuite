using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEHostilityManagerData))]
	public class MEHostilityManager : HostilityManager, IEntityData<FrostySdk.Ebx.MEHostilityManagerData>
	{
		public new FrostySdk.Ebx.MEHostilityManagerData Data => data as FrostySdk.Ebx.MEHostilityManagerData;
		public override string DisplayName => "MEHostilityManager";

		public MEHostilityManager(FrostySdk.Ebx.MEHostilityManagerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


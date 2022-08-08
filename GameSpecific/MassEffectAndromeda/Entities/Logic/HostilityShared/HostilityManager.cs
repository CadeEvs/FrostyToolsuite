using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HostilityManagerData))]
	public class HostilityManager : LogicEntity, IEntityData<FrostySdk.Ebx.HostilityManagerData>
	{
		public new FrostySdk.Ebx.HostilityManagerData Data => data as FrostySdk.Ebx.HostilityManagerData;
		public override string DisplayName => "HostilityManager";

		public HostilityManager(FrostySdk.Ebx.HostilityManagerData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


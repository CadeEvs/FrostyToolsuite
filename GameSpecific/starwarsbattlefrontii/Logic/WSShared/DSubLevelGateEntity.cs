using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DSubLevelGateEntityData))]
	public class DSubLevelGateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DSubLevelGateEntityData>
	{
		public new FrostySdk.Ebx.DSubLevelGateEntityData Data => data as FrostySdk.Ebx.DSubLevelGateEntityData;
		public override string DisplayName => "DSubLevelGate";

		public DSubLevelGateEntity(FrostySdk.Ebx.DSubLevelGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


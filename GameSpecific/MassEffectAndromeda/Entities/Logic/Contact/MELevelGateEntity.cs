using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MELevelGateEntityData))]
	public class MELevelGateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MELevelGateEntityData>
	{
		public new FrostySdk.Ebx.MELevelGateEntityData Data => data as FrostySdk.Ebx.MELevelGateEntityData;
		public override string DisplayName => "MELevelGate";

		public MELevelGateEntity(FrostySdk.Ebx.MELevelGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


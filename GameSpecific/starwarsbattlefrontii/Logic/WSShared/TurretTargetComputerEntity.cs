using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TurretTargetComputerEntityData))]
	public class TurretTargetComputerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TurretTargetComputerEntityData>
	{
		public new FrostySdk.Ebx.TurretTargetComputerEntityData Data => data as FrostySdk.Ebx.TurretTargetComputerEntityData;
		public override string DisplayName => "TurretTargetComputer";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TurretTargetComputerEntity(FrostySdk.Ebx.TurretTargetComputerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


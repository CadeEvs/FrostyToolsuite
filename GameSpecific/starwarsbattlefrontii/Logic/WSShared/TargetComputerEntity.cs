using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TargetComputerEntityData))]
	public class TargetComputerEntity : AimEntityBase, IEntityData<FrostySdk.Ebx.TargetComputerEntityData>
	{
		public new FrostySdk.Ebx.TargetComputerEntityData Data => data as FrostySdk.Ebx.TargetComputerEntityData;
		public override string DisplayName => "TargetComputer";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public TargetComputerEntity(FrostySdk.Ebx.TargetComputerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


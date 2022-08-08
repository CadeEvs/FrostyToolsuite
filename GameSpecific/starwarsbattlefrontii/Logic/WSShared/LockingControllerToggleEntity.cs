using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LockingControllerToggleEntityData))]
	public class LockingControllerToggleEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LockingControllerToggleEntityData>
	{
		public new FrostySdk.Ebx.LockingControllerToggleEntityData Data => data as FrostySdk.Ebx.LockingControllerToggleEntityData;
		public override string DisplayName => "LockingControllerToggle";

		public LockingControllerToggleEntity(FrostySdk.Ebx.LockingControllerToggleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


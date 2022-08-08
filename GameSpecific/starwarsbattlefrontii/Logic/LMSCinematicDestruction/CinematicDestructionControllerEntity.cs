using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinematicDestructionControllerEntityData))]
	public class CinematicDestructionControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CinematicDestructionControllerEntityData>
	{
		public new FrostySdk.Ebx.CinematicDestructionControllerEntityData Data => data as FrostySdk.Ebx.CinematicDestructionControllerEntityData;
		public override string DisplayName => "CinematicDestructionController";

		public CinematicDestructionControllerEntity(FrostySdk.Ebx.CinematicDestructionControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


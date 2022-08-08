using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinematicDestructionOutputPipeEntityData))]
	public class CinematicDestructionOutputPipeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CinematicDestructionOutputPipeEntityData>
	{
		public new FrostySdk.Ebx.CinematicDestructionOutputPipeEntityData Data => data as FrostySdk.Ebx.CinematicDestructionOutputPipeEntityData;
		public override string DisplayName => "CinematicDestructionOutputPipe";

		public CinematicDestructionOutputPipeEntity(FrostySdk.Ebx.CinematicDestructionOutputPipeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


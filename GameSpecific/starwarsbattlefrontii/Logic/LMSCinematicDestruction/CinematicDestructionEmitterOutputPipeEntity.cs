using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinematicDestructionEmitterOutputPipeEntityData))]
	public class CinematicDestructionEmitterOutputPipeEntity : CinematicDestructionOutputPipeEntity, IEntityData<FrostySdk.Ebx.CinematicDestructionEmitterOutputPipeEntityData>
	{
		public new FrostySdk.Ebx.CinematicDestructionEmitterOutputPipeEntityData Data => data as FrostySdk.Ebx.CinematicDestructionEmitterOutputPipeEntityData;
		public override string DisplayName => "CinematicDestructionEmitterOutputPipe";

		public CinematicDestructionEmitterOutputPipeEntity(FrostySdk.Ebx.CinematicDestructionEmitterOutputPipeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


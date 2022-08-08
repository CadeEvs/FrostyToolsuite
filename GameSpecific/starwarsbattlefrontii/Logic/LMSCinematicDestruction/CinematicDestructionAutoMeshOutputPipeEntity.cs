using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinematicDestructionAutoMeshOutputPipeEntityData))]
	public class CinematicDestructionAutoMeshOutputPipeEntity : CinematicDestructionMeshOutputPipeEntity, IEntityData<FrostySdk.Ebx.CinematicDestructionAutoMeshOutputPipeEntityData>
	{
		public new FrostySdk.Ebx.CinematicDestructionAutoMeshOutputPipeEntityData Data => data as FrostySdk.Ebx.CinematicDestructionAutoMeshOutputPipeEntityData;
		public override string DisplayName => "CinematicDestructionAutoMeshOutputPipe";

		public CinematicDestructionAutoMeshOutputPipeEntity(FrostySdk.Ebx.CinematicDestructionAutoMeshOutputPipeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


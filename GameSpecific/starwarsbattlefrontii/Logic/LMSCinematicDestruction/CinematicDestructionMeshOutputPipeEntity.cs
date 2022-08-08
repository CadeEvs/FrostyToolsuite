using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinematicDestructionMeshOutputPipeEntityData))]
	public class CinematicDestructionMeshOutputPipeEntity : CinematicDestructionOutputPipeEntity, IEntityData<FrostySdk.Ebx.CinematicDestructionMeshOutputPipeEntityData>
	{
		public new FrostySdk.Ebx.CinematicDestructionMeshOutputPipeEntityData Data => data as FrostySdk.Ebx.CinematicDestructionMeshOutputPipeEntityData;
		public override string DisplayName => "CinematicDestructionMeshOutputPipe";

		public CinematicDestructionMeshOutputPipeEntity(FrostySdk.Ebx.CinematicDestructionMeshOutputPipeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


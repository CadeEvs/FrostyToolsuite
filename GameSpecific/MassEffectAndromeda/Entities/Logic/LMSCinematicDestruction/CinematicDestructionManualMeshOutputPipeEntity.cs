using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinematicDestructionManualMeshOutputPipeEntityData))]
	public class CinematicDestructionManualMeshOutputPipeEntity : CinematicDestructionMeshOutputPipeEntity, IEntityData<FrostySdk.Ebx.CinematicDestructionManualMeshOutputPipeEntityData>
	{
		public new FrostySdk.Ebx.CinematicDestructionManualMeshOutputPipeEntityData Data => data as FrostySdk.Ebx.CinematicDestructionManualMeshOutputPipeEntityData;
		public override string DisplayName => "CinematicDestructionManualMeshOutputPipe";

		public CinematicDestructionManualMeshOutputPipeEntity(FrostySdk.Ebx.CinematicDestructionManualMeshOutputPipeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


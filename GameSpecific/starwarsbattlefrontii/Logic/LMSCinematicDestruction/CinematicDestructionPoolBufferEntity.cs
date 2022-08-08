using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinematicDestructionPoolBufferEntityData))]
	public class CinematicDestructionPoolBufferEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CinematicDestructionPoolBufferEntityData>
	{
		public new FrostySdk.Ebx.CinematicDestructionPoolBufferEntityData Data => data as FrostySdk.Ebx.CinematicDestructionPoolBufferEntityData;
		public override string DisplayName => "CinematicDestructionPoolBuffer";

		public CinematicDestructionPoolBufferEntity(FrostySdk.Ebx.CinematicDestructionPoolBufferEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


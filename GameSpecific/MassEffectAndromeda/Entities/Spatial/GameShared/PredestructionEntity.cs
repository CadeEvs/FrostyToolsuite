using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PredestructionEntityData))]
	public class PredestructionEntity : SpatialEntity, IEntityData<FrostySdk.Ebx.PredestructionEntityData>
	{
		public new FrostySdk.Ebx.PredestructionEntityData Data => data as FrostySdk.Ebx.PredestructionEntityData;

		public PredestructionEntity(FrostySdk.Ebx.PredestructionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BreakableModelEntityData))]
	public class BreakableModelEntity : GamePhysicsEntity, IEntityData<FrostySdk.Ebx.BreakableModelEntityData>
	{
		public new FrostySdk.Ebx.BreakableModelEntityData Data => data as FrostySdk.Ebx.BreakableModelEntityData;

		public BreakableModelEntity(FrostySdk.Ebx.BreakableModelEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ControllableEntityData))]
	public class ControllableEntity : GamePhysicsEntity, IEntityData<FrostySdk.Ebx.ControllableEntityData>
	{
		public new FrostySdk.Ebx.ControllableEntityData Data => data as FrostySdk.Ebx.ControllableEntityData;

		public ControllableEntity(FrostySdk.Ebx.ControllableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


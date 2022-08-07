using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GamePhysicsEntityData))]
	public class GamePhysicsEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.GamePhysicsEntityData>
	{
		public new FrostySdk.Ebx.GamePhysicsEntityData Data => data as FrostySdk.Ebx.GamePhysicsEntityData;

		public GamePhysicsEntity(FrostySdk.Ebx.GamePhysicsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


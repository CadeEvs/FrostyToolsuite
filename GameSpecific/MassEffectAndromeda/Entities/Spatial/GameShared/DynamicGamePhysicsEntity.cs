using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicGamePhysicsEntityData))]
	public class DynamicGamePhysicsEntity : GamePhysicsEntity, IEntityData<FrostySdk.Ebx.DynamicGamePhysicsEntityData>
	{
		public new FrostySdk.Ebx.DynamicGamePhysicsEntityData Data => data as FrostySdk.Ebx.DynamicGamePhysicsEntityData;

		public DynamicGamePhysicsEntity(FrostySdk.Ebx.DynamicGamePhysicsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


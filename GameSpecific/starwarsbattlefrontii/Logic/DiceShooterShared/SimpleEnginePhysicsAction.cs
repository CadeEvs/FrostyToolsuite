using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SimpleEnginePhysicsActionData))]
	public class SimpleEnginePhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.SimpleEnginePhysicsActionData>
	{
		public new FrostySdk.Ebx.SimpleEnginePhysicsActionData Data => data as FrostySdk.Ebx.SimpleEnginePhysicsActionData;
		public override string DisplayName => "SimpleEnginePhysicsAction";

		public SimpleEnginePhysicsAction(FrostySdk.Ebx.SimpleEnginePhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


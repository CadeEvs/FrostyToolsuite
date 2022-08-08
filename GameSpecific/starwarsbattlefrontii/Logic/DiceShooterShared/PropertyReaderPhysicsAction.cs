using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyReaderPhysicsActionData))]
	public class PropertyReaderPhysicsAction : PhysicsActionBase, IEntityData<FrostySdk.Ebx.PropertyReaderPhysicsActionData>
	{
		public new FrostySdk.Ebx.PropertyReaderPhysicsActionData Data => data as FrostySdk.Ebx.PropertyReaderPhysicsActionData;
		public override string DisplayName => "PropertyReaderPhysicsAction";

		public PropertyReaderPhysicsAction(FrostySdk.Ebx.PropertyReaderPhysicsActionData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


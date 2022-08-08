using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PersistentTransformVariableEntityData))]
	public class PersistentTransformVariableEntity : PersistentVariableEntityBase, IEntityData<FrostySdk.Ebx.PersistentTransformVariableEntityData>
	{
		public new FrostySdk.Ebx.PersistentTransformVariableEntityData Data => data as FrostySdk.Ebx.PersistentTransformVariableEntityData;
		public override string DisplayName => "PersistentTransformVariable";

		public PersistentTransformVariableEntity(FrostySdk.Ebx.PersistentTransformVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


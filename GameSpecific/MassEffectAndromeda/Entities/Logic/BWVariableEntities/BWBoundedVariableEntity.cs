using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWBoundedVariableEntityData))]
	public class BWBoundedVariableEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BWBoundedVariableEntityData>
	{
		public new FrostySdk.Ebx.BWBoundedVariableEntityData Data => data as FrostySdk.Ebx.BWBoundedVariableEntityData;
		public override string DisplayName => "BWBoundedVariable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BWBoundedVariableEntity(FrostySdk.Ebx.BWBoundedVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


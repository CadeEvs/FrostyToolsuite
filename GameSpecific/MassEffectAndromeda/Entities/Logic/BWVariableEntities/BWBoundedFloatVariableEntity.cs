using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWBoundedFloatVariableEntityData))]
	public class BWBoundedFloatVariableEntity : BWBoundedVariableEntity, IEntityData<FrostySdk.Ebx.BWBoundedFloatVariableEntityData>
	{
		public new FrostySdk.Ebx.BWBoundedFloatVariableEntityData Data => data as FrostySdk.Ebx.BWBoundedFloatVariableEntityData;
		public override string DisplayName => "BWBoundedFloatVariable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BWBoundedFloatVariableEntity(FrostySdk.Ebx.BWBoundedFloatVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


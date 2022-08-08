using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DynamicDebugVariableEntityData))]
	public class DynamicDebugVariableEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DynamicDebugVariableEntityData>
	{
		public new FrostySdk.Ebx.DynamicDebugVariableEntityData Data => data as FrostySdk.Ebx.DynamicDebugVariableEntityData;
		public override string DisplayName => "DynamicDebugVariable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public DynamicDebugVariableEntity(FrostySdk.Ebx.DynamicDebugVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


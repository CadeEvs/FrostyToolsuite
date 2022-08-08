using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWVec4VariableEntityData))]
	public class BWVec4VariableEntity : BWVariableEntityBase, IEntityData<FrostySdk.Ebx.BWVec4VariableEntityData>
	{
		public new FrostySdk.Ebx.BWVec4VariableEntityData Data => data as FrostySdk.Ebx.BWVec4VariableEntityData;
		public override string DisplayName => "BWVec4Variable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("InputValue", Direction.In),
				new ConnectionDesc("OutputValue", Direction.Out)
			};
		}

		public BWVec4VariableEntity(FrostySdk.Ebx.BWVec4VariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


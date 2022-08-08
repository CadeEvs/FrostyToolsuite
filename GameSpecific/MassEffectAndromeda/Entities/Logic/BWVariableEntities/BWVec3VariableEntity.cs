using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWVec3VariableEntityData))]
	public class BWVec3VariableEntity : BWVariableEntityBase, IEntityData<FrostySdk.Ebx.BWVec3VariableEntityData>
	{
		public new FrostySdk.Ebx.BWVec3VariableEntityData Data => data as FrostySdk.Ebx.BWVec3VariableEntityData;
		public override string DisplayName => "BWVec3Variable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("InputValue", Direction.In),
				new ConnectionDesc("OutputValue", Direction.Out)
			};
		}

		public BWVec3VariableEntity(FrostySdk.Ebx.BWVec3VariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


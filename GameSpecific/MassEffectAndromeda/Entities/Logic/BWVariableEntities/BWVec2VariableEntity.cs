using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWVec2VariableEntityData))]
	public class BWVec2VariableEntity : BWVariableEntityBase, IEntityData<FrostySdk.Ebx.BWVec2VariableEntityData>
	{
		public new FrostySdk.Ebx.BWVec2VariableEntityData Data => data as FrostySdk.Ebx.BWVec2VariableEntityData;
		public override string DisplayName => "BWVec2Variable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("InputValue", Direction.In),
				new ConnectionDesc("OutputValue", Direction.Out)
			};
		}

		public BWVec2VariableEntity(FrostySdk.Ebx.BWVec2VariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


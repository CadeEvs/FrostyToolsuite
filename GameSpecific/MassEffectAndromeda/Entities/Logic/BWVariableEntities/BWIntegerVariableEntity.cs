using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWIntegerVariableEntityData))]
	public class BWIntegerVariableEntity : BWVariableEntityBase, IEntityData<FrostySdk.Ebx.BWIntegerVariableEntityData>
	{
		public new FrostySdk.Ebx.BWIntegerVariableEntityData Data => data as FrostySdk.Ebx.BWIntegerVariableEntityData;
		public override string DisplayName => "BWIntegerVariable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("InputValue", Direction.In),
				new ConnectionDesc("OutputValue", Direction.Out)
			};
		}

		public BWIntegerVariableEntity(FrostySdk.Ebx.BWIntegerVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


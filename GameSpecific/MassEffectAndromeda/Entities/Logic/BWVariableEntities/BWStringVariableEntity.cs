using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWStringVariableEntityData))]
	public class BWStringVariableEntity : BWVariableEntityBase, IEntityData<FrostySdk.Ebx.BWStringVariableEntityData>
	{
		public new FrostySdk.Ebx.BWStringVariableEntityData Data => data as FrostySdk.Ebx.BWStringVariableEntityData;
		public override string DisplayName => "BWStringVariable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("InputValue", Direction.In),
				new ConnectionDesc("OutputValue", Direction.Out)
			};
		}

		public BWStringVariableEntity(FrostySdk.Ebx.BWStringVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


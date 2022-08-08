using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWTransformVariableEntityData))]
	public class BWTransformVariableEntity : BWVariableEntityBase, IEntityData<FrostySdk.Ebx.BWTransformVariableEntityData>
	{
		public new FrostySdk.Ebx.BWTransformVariableEntityData Data => data as FrostySdk.Ebx.BWTransformVariableEntityData;
		public override string DisplayName => "BWTransformVariable";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("InputValue", Direction.In),
				new ConnectionDesc("OutputValue", Direction.Out)
			};
		}

		public BWTransformVariableEntity(FrostySdk.Ebx.BWTransformVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


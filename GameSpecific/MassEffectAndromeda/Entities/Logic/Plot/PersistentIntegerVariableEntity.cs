using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PersistentIntegerVariableEntityData))]
	public class PersistentIntegerVariableEntity : PersistentVariableEntityBase, IEntityData<FrostySdk.Ebx.PersistentIntegerVariableEntityData>
	{
		public new FrostySdk.Ebx.PersistentIntegerVariableEntityData Data => data as FrostySdk.Ebx.PersistentIntegerVariableEntityData;
		public override string DisplayName => "PersistentIntegerVariable";
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("InputValue", Direction.In),
				new ConnectionDesc("OutputValue", Direction.Out)
			};
		}

		public PersistentIntegerVariableEntity(FrostySdk.Ebx.PersistentIntegerVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


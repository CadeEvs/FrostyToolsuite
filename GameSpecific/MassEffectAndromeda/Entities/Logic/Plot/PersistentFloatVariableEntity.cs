using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PersistentFloatVariableEntityData))]
	public class PersistentFloatVariableEntity : PersistentVariableEntityBase, IEntityData<FrostySdk.Ebx.PersistentFloatVariableEntityData>
	{
		public new FrostySdk.Ebx.PersistentFloatVariableEntityData Data => data as FrostySdk.Ebx.PersistentFloatVariableEntityData;
		public override string DisplayName => "PersistentFloatVariable";
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("InputValue", Direction.In),
				new ConnectionDesc("OutputValue", Direction.Out)
			};
		}

		public PersistentFloatVariableEntity(FrostySdk.Ebx.PersistentFloatVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


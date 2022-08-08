using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PersistentVariableEntityBaseData))]
	public class PersistentVariableEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.PersistentVariableEntityBaseData>
	{
		public new FrostySdk.Ebx.PersistentVariableEntityBaseData Data => data as FrostySdk.Ebx.PersistentVariableEntityBaseData;
		public override string DisplayName => "PersistentVariableEntityBase";
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Get", Direction.In),
				new ConnectionDesc("Set", Direction.In),
				new ConnectionDesc("OnGet", Direction.Out),
				new ConnectionDesc("OnSet", Direction.Out)
			};
		}

        public PersistentVariableEntityBase(FrostySdk.Ebx.PersistentVariableEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


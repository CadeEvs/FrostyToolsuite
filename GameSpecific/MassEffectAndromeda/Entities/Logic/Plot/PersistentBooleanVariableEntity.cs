using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PersistentBooleanVariableEntityData))]
	public class PersistentBooleanVariableEntity : PersistentVariableEntityBase, IEntityData<FrostySdk.Ebx.PersistentBooleanVariableEntityData>
	{
		public new FrostySdk.Ebx.PersistentBooleanVariableEntityData Data => data as FrostySdk.Ebx.PersistentBooleanVariableEntityData;
		public override string DisplayName => "PersistentBooleanVariable";
        public override IEnumerable<ConnectionDesc> Events
        {
            get
            {
                List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
                outEvents.AddRange(base.Events);
                outEvents.AddRange(new List<ConnectionDesc>()
                {
                    new ConnectionDesc("SetTrue", Direction.In),
                    new ConnectionDesc("SetFalse", Direction.In),
                    new ConnectionDesc("OnTrue", Direction.Out),
                    new ConnectionDesc("OnFalse", Direction.Out),
                    new ConnectionDesc("OnChange", Direction.Out)
                });
                return outEvents;
            }
        }

        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("InputValue", Direction.In),
                new ConnectionDesc("OutputValue", Direction.Out)
            };
        }

        public PersistentBooleanVariableEntity(FrostySdk.Ebx.PersistentBooleanVariableEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


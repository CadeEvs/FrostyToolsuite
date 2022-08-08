using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntegerSwitchCaseEntityData))]
	public class IntegerSwitchCaseEntity : BaseSwitchCaseEntity, IEntityData<FrostySdk.Ebx.IntegerSwitchCaseEntityData>
	{
		public new FrostySdk.Ebx.IntegerSwitchCaseEntityData Data => data as FrostySdk.Ebx.IntegerSwitchCaseEntityData;
		public override string DisplayName => "IntegerSwitchCase";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
			get
            {
				List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
				outEvents.AddRange(base.Events);

				for (int i = 0; i < Data.Cases.Count; i++)
                {
					outEvents.Add(new ConnectionDesc() { Name = $"Case {Data.Cases[i]}", Direction = Direction.Out });
                }

				return outEvents;
            }
        }
        public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("InValue", Direction.In)
			};
		}

		public IntegerSwitchCaseEntity(FrostySdk.Ebx.IntegerSwitchCaseEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


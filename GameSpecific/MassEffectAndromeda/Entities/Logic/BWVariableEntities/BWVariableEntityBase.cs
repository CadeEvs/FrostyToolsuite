using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWVariableEntityBaseData))]
	public class BWVariableEntityBase : LogicEntity, IEntityData<FrostySdk.Ebx.BWVariableEntityBaseData>
	{
		protected readonly int Event_Set = Frosty.Hash.Fnv1.HashString("Set");
		protected readonly int Event_Get = Frosty.Hash.Fnv1.HashString("Get");
		protected readonly int Event_OnSet = Frosty.Hash.Fnv1.HashString("OnSet");
		protected readonly int Event_OnChange = Frosty.Hash.Fnv1.HashString("OnChange");

		public new FrostySdk.Ebx.BWVariableEntityBaseData Data => data as FrostySdk.Ebx.BWVariableEntityBaseData;
		public override string DisplayName => "BWVariableEntityBase";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Set", Direction.In),
				new ConnectionDesc("Get", Direction.In),
				new ConnectionDesc("OnSet", Direction.Out),
				new ConnectionDesc("OnChange", Direction.Out),
			};
		}

		protected Event<InputEvent> setEvent;
		protected Event<InputEvent> getEvent;
		protected Event<OutputEvent> onSetEvent;
		protected Event<OutputEvent> onChangeEvent;

		public BWVariableEntityBase(FrostySdk.Ebx.BWVariableEntityBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
			setEvent = new Event<InputEvent>(this, Event_Set);
			getEvent = new Event<InputEvent>(this, Event_Get);
			onSetEvent = new Event<OutputEvent>(this, Event_OnSet);
			onChangeEvent = new Event<OutputEvent>(this, Event_OnChange);
		}
	}
}


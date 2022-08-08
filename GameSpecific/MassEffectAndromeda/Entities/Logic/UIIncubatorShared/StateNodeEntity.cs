using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StateNodeEntityData))]
	public class StateNodeEntity : StateNodeEntityBase, IEntityData<FrostySdk.Ebx.StateNodeEntityData>
	{
		protected readonly int Link_InitialState = Frosty.Hash.Fnv1.HashString("InitialState");
		protected readonly int Link_ChildStates = Frosty.Hash.Fnv1.HashString("ChildStates");

		protected readonly int Event_Enter = Frosty.Hash.Fnv1.HashString("Enter");
		protected readonly int Event_OnEnter = Frosty.Hash.Fnv1.HashString("OnEnter");
		protected readonly int Event_OnExit = Frosty.Hash.Fnv1.HashString("OnExit");

		protected readonly int Property_Active = Frosty.Hash.Fnv1.HashString("Active");

		public new FrostySdk.Ebx.StateNodeEntityData Data => data as FrostySdk.Ebx.StateNodeEntityData;
		public override string DisplayName => "StateNode";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Enter", Direction.In),
				new ConnectionDesc("OnEnter", Direction.Out),
				new ConnectionDesc("OnExit", Direction.Out)
			};
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Active", Direction.Out, typeof(bool))
			};
        }
        public override IEnumerable<ConnectionDesc> Links
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("InitialState", Direction.In, typeof(StateNodeEntity)),
				new ConnectionDesc("ChildStates", Direction.In, typeof(List<StateNodeEntity>))
			};
        }
        public override IEnumerable<string> DebugRows
        {
			get
            {
				List<string> outDebugRows = new List<string>();
				if (rootState == null && currentState != null)
				{
					outDebugRows.Add($"CurrentState: {currentState.Data.StateName}");
				}
				return outDebugRows;
            }
        }
        public bool Active
        {
			get => activeProperty.Value;
			set => activeProperty.Value = value;
        }

		protected Link<StateNodeEntity> initialStateLink;
		protected LinkArray<StateNodeEntity> childStatesLink;
		protected Event<InputEvent> enterEvent;
		protected Event<OutputEvent> onEnterEvent;
		protected Event<OutputEvent> onExitEvent;
		protected Property<bool> activeProperty;

		protected StateNodeEntity currentState;
		protected StateNodeEntity rootState;

        public StateNodeEntity(FrostySdk.Ebx.StateNodeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			initialStateLink = new Link<StateNodeEntity>(this, Link_InitialState);
			childStatesLink = new LinkArray<StateNodeEntity>(this, Link_ChildStates);
			enterEvent = new Event<InputEvent>(this, Event_Enter);
			onEnterEvent = new Event<OutputEvent>(this, Event_OnEnter);
			onExitEvent = new Event<OutputEvent>(this, Event_OnExit);
			activeProperty = new Property<bool>(this, Property_Active);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();
			activeProperty.Value = false;
        }

        public override void OnLinkChanged(int linkHash)
        {
			if (linkHash == initialStateLink.NameHash)
			{
				currentState = initialStateLink.Value;
				currentState.Active = true;
				currentState.enterEvent.Execute();
				return;
			}
            else if (linkHash == childStatesLink.NameHash)
            {
				childStatesLink.Value[childStatesLink.Value.Count - 1].rootState = this;
                return;
            }

            base.OnLinkChanged(linkHash);
        }

        public override void OnEvent(int eventHash)
        {
			if (eventHash == enterEvent.NameHash)
			{
				rootState.currentState.onExitEvent.Execute();
				rootState.currentState.activeProperty.Value = false;
				rootState.currentState = this;
				activeProperty.Value = true;
				onEnterEvent.Execute();
			}

            base.OnEvent(eventHash);
        }
    }
}


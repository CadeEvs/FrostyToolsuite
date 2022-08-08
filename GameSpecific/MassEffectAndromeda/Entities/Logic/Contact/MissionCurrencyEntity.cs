using System.Collections.Generic;
using CString = FrostySdk.Ebx.CString;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MissionCurrencyEntityData))]
	public class MissionCurrencyEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MissionCurrencyEntityData>
	{
		protected readonly int Property_MissionCurrency = Frosty.Hash.Fnv1.HashString("MissionCurrency");

		public new FrostySdk.Ebx.MissionCurrencyEntityData Data => data as FrostySdk.Ebx.MissionCurrencyEntityData;
		public override string DisplayName => "MissionCurrency";
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("MissionCurrency", Direction.Out, typeof(CString))
			};
        }

		protected Property<CString> missionCurrencyProperty;
		protected Event<InputEvent> event_208e92b5;

        public MissionCurrencyEntity(FrostySdk.Ebx.MissionCurrencyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			missionCurrencyProperty = new Property<CString>(this, Property_MissionCurrency);
			event_208e92b5 = new Event<InputEvent>(this, 0x208e92b5);
		}

        public override void OnEvent(int eventHash)
        {
			if (eventHash == event_208e92b5.NameHash)
			{
				missionCurrencyProperty.Value = "999,999";
				return;
			}

            base.OnEvent(eventHash);
        }
    }
}


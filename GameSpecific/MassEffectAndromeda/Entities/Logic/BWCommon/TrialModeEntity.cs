using FrostySdk.Attributes;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TrialModeEntityData))]
	public class TrialModeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TrialModeEntityData>
	{
		private class TrialModeMockData
		{
			[EbxFieldMeta(FrostySdk.IO.EbxFieldType.Boolean)]
			public bool IsTrial { get; set; }
			[EbxFieldMeta(FrostySdk.IO.EbxFieldType.Boolean)]
			public bool IsTrialExpired { get; set; }
			[EbxFieldMeta(FrostySdk.IO.EbxFieldType.Float32)]
			public float TrialTimeLeft { get; set; }
		}

		public new FrostySdk.Ebx.TrialModeEntityData Data => data as FrostySdk.Ebx.TrialModeEntityData;
		public override string DisplayName => "TrialMode";

		protected Property<bool> property_4aca24de;
		protected Property<bool> property_ec7f49aa;
		protected Property<float> property_be657b81;

		protected Event<InputEvent> event_0231bc97;

		// static so that all trial mode entities share the same mocked data
		private static TrialModeMockData staticMockData = new TrialModeMockData();

		public TrialModeEntity(FrostySdk.Ebx.TrialModeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			mockDataObject = staticMockData;

			property_4aca24de = new Property<bool>(this, 0x4aca24de);
			property_ec7f49aa = new Property<bool>(this, -327202390);
			property_be657b81 = new Property<float>(this, -1100645503);

			event_0231bc97 = new Event<InputEvent>(this, 0x0231bc97);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();

			property_4aca24de.Value = (mockDataObject as TrialModeMockData).IsTrial;
			property_ec7f49aa.Value = (mockDataObject as TrialModeMockData).IsTrialExpired;
			property_be657b81.Value = (mockDataObject as TrialModeMockData).TrialTimeLeft;
		}

        public override void OnEvent(int eventHash)
        {
			if (eventHash == event_0231bc97.NameHash)
			{
				property_4aca24de.Value = (mockDataObject as TrialModeMockData).IsTrial;
				property_ec7f49aa.Value = (mockDataObject as TrialModeMockData).IsTrialExpired;
				property_be657b81.Value = (mockDataObject as TrialModeMockData).TrialTimeLeft;
				return;
			}

            base.OnEvent(eventHash);
        }
    }
}


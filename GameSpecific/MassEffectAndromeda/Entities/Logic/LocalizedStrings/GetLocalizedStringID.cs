using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GetLocalizedStringIDData))]
	public class GetLocalizedStringID : LogicEntity, IEntityData<FrostySdk.Ebx.GetLocalizedStringIDData>
	{
		protected readonly int Property_StringIDOverride = Frosty.Hash.Fnv1.HashString("StringIDOverride");

		public new FrostySdk.Ebx.GetLocalizedStringIDData Data => data as FrostySdk.Ebx.GetLocalizedStringIDData;
		public override string DisplayName => "GetLocalizedStringID";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("StringIDOverride", Direction.In)
			};
		}

		protected Property<int> stringIdOverrideProperty;
		protected Property<int> property_822a5fbd;

		public GetLocalizedStringID(FrostySdk.Ebx.GetLocalizedStringIDData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			stringIdOverrideProperty = new Property<int>(this, Property_StringIDOverride);
			property_822a5fbd = new Property<int>(this, -2111152195);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();
			if (property_822a5fbd.IsUnset)
			{
				property_822a5fbd.Value = Data.Text.StringId;
			}
        }

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == stringIdOverrideProperty.NameHash)
			{
				property_822a5fbd.Value = stringIdOverrideProperty.Value;
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }
    }
}


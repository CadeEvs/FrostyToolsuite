using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.FloatEntityData))]
	public class FloatEntity : LogicEntity, IEntityData<FrostySdk.Ebx.FloatEntityData>
	{
		protected readonly int Property_Value = Frosty.Hash.Fnv1.HashString("Value");

		public new FrostySdk.Ebx.FloatEntityData Data => data as FrostySdk.Ebx.FloatEntityData;
		public override string DisplayName => "Float";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Value", Direction.Out, typeof(float))
			};
		}
        public override IEnumerable<string> HeaderRows
        {
			get => new List<string>()
			{
				$"DefaultValue: {Data.DefaultValue}"
			};
        }

		protected Property<float> valueProperty;

        public FloatEntity(FrostySdk.Ebx.FloatEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			valueProperty = new Property<float>(this, Property_Value);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();

			if (valueProperty.IsUnset)
			{
				valueProperty.Value = Data.DefaultValue;
			}
        }
    }
}


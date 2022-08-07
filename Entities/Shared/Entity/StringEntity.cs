using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StringEntityData))]
	public class StringEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StringEntityData>
	{
		protected readonly int Property_String = Frosty.Hash.Fnv1.HashString("String");

		public new FrostySdk.Ebx.StringEntityData Data => data as FrostySdk.Ebx.StringEntityData;
		public override string DisplayName => "String";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("DefaultString", Direction.In),
				new ConnectionDesc("String", Direction.Out)
			};
		}

		protected Property<FrostySdk.Ebx.CString> stringProperty;

		public StringEntity(FrostySdk.Ebx.StringEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			stringProperty = new Property<FrostySdk.Ebx.CString>(this, Property_String);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();

			if (stringProperty.IsUnset)
			{
				stringProperty.Value = Data.DefaultString;
			}
		}
    }
}


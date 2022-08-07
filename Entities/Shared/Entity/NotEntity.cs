using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NotEntityData))]
	public class NotEntity : LogicEntity, IEntityData<FrostySdk.Ebx.NotEntityData>
	{
		protected readonly int Property_In = Frosty.Hash.Fnv1.HashString("In");
		protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.NotEntityData Data => data as FrostySdk.Ebx.NotEntityData;
		public override string DisplayName => "Not";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In", Direction.In, typeof(bool)),
				new ConnectionDesc("Out", Direction.Out, typeof(bool)),
			};
		}

		protected Property<bool> inProperty;
		protected Property<bool> outProperty;

		public NotEntity(FrostySdk.Ebx.NotEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			inProperty = new Property<bool>(this, Property_In, Data.In);
			outProperty = new Property<bool>(this, Property_Out);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == inProperty.NameHash)
			{
				outProperty.Value = !inProperty.Value;
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }
    }
}


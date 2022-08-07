using System.Collections.Generic;
using Vec3 = FrostySdk.Ebx.Vec3;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec3BuilderEntityData))]
	public class Vec3BuilderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.Vec3BuilderEntityData>
	{
		protected readonly int Property_X = Frosty.Hash.Fnv1.HashString("X");
		protected readonly int Property_Y = Frosty.Hash.Fnv1.HashString("Y");
		protected readonly int Property_Z = Frosty.Hash.Fnv1.HashString("Z");
		protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.Vec3BuilderEntityData Data => data as FrostySdk.Ebx.Vec3BuilderEntityData;
		public override string DisplayName => "Vec3Builder";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("X", Direction.In, typeof(float)),
				new ConnectionDesc("Y", Direction.In, typeof(float)),
				new ConnectionDesc("Z", Direction.In, typeof(float)),
				new ConnectionDesc("Out", Direction.Out, typeof(Vec3))
			};
		}

		protected Property<float> xProperty;
		protected Property<float> yProperty;
		protected Property<float> zProperty;
		protected Property<Vec3> outProperty;

		public Vec3BuilderEntity(FrostySdk.Ebx.Vec3BuilderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			xProperty = new Property<float>(this, Property_X, 0.0f);
			yProperty = new Property<float>(this, Property_Y, 0.0f);
			zProperty = new Property<float>(this, Property_Z, 0.0f);
			outProperty = new Property<Vec3>(this, Property_Out);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == xProperty.NameHash || propertyHash == yProperty.NameHash || propertyHash == zProperty.NameHash)
			{
				outProperty.Value = new Vec3()
				{
					x = xProperty.Value,
					y = yProperty.Value,
					z = zProperty.Value
				};
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }
    }
}


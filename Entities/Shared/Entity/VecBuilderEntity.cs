using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VecBuilderEntityData))]
	public class VecBuilderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VecBuilderEntityData>
	{
		protected readonly int Property_X = Frosty.Hash.Fnv1.HashString("X");
		protected readonly int Property_Y = Frosty.Hash.Fnv1.HashString("Y");
		protected readonly int Property_Z = Frosty.Hash.Fnv1.HashString("Z");
		protected readonly int Property_W = Frosty.Hash.Fnv1.HashString("W");
		protected readonly int Property_Vec3 = Frosty.Hash.Fnv1.HashString("Vec3");

		public new FrostySdk.Ebx.VecBuilderEntityData Data => data as FrostySdk.Ebx.VecBuilderEntityData;
		public override string DisplayName => "VecBuilder";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("X", Direction.In, typeof(float)),
				new ConnectionDesc("Y", Direction.In, typeof(float)),
				new ConnectionDesc("Z", Direction.In, typeof(float)),
				new ConnectionDesc("W", Direction.In, typeof(float)),
				new ConnectionDesc("Vec3", Direction.Out, typeof(FrostySdk.Ebx.Vec3))
			};
        }

		protected Property<float> xProperty;
		protected Property<float> yProperty;
		protected Property<float> zProperty;
		protected Property<float> wProperty;
		protected Property<FrostySdk.Ebx.Vec3> vec3Property;

		public VecBuilderEntity(FrostySdk.Ebx.VecBuilderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			xProperty = new Property<float>(this, Property_X, Data.X);
			yProperty = new Property<float>(this, Property_Y, Data.Y);
			zProperty = new Property<float>(this, Property_Z, Data.Z);
			wProperty = new Property<float>(this, Property_W, Data.W);
			vec3Property = new Property<FrostySdk.Ebx.Vec3>(this, Property_Vec3, new FrostySdk.Ebx.Vec3());
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == xProperty.NameHash || propertyHash == yProperty.NameHash || propertyHash == zProperty.NameHash || propertyHash == wProperty.NameHash)
			{
				vec3Property.Value = new FrostySdk.Ebx.Vec3() { x = xProperty.Value, y = yProperty.Value, z = zProperty.Value };
				return;
			}

			base.OnPropertyChanged(propertyHash);
        }
    }
}


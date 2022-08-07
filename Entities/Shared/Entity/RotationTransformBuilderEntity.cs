using System.Collections.Generic;
using LinearTransform = FrostySdk.Ebx.LinearTransform;
using Vec3 = FrostySdk.Ebx.Vec3;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RotationTransformBuilderEntityData))]
	public class RotationTransformBuilderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RotationTransformBuilderEntityData>
	{
		protected readonly int Property_Rotation = Frosty.Hash.Fnv1.HashString("Rotation");
		protected readonly int Property_Transform = Frosty.Hash.Fnv1.HashString("Transform");

		public new FrostySdk.Ebx.RotationTransformBuilderEntityData Data => data as FrostySdk.Ebx.RotationTransformBuilderEntityData;
		public override string DisplayName => "RotationTransformBuilder";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Rotation", Direction.In),
				new ConnectionDesc("Transform", Direction.Out)
			};
        }

		protected Property<Vec3> rotationProperty;
		protected Property<LinearTransform> transformProperty;

        public RotationTransformBuilderEntity(FrostySdk.Ebx.RotationTransformBuilderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			rotationProperty = new Property<Vec3>(this, Property_Rotation);
			transformProperty = new Property<LinearTransform>(this, Property_Transform);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == rotationProperty.NameHash)
            {
				var value = rotationProperty.Value;
				Matrix m = Matrix.RotationX(value.x) * Matrix.RotationY(value.y) * Matrix.RotationZ(value.z);
				transformProperty.Value = MakeLinearTransform(m);
				return;
            }
            base.OnPropertyChanged(propertyHash);
        }
    }
}


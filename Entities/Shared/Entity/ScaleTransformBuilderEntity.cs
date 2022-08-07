using System.Collections.Generic;
using LinearTransform = FrostySdk.Ebx.LinearTransform;
using Vec3 = FrostySdk.Ebx.Vec3;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScaleTransformBuilderEntityData))]
	public class ScaleTransformBuilderEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScaleTransformBuilderEntityData>
	{
		protected readonly int Property_Scale = Frosty.Hash.Fnv1.HashString("Scale");
		protected readonly int Property_Transform = Frosty.Hash.Fnv1.HashString("Transform");

		public new FrostySdk.Ebx.ScaleTransformBuilderEntityData Data => data as FrostySdk.Ebx.ScaleTransformBuilderEntityData;
		public override string DisplayName => "ScaleTransformBuilder";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Scale", Direction.In, typeof(Vec3)),
				new ConnectionDesc("Transform", Direction.Out, typeof(LinearTransform))
			};
        }

		protected Property<Vec3> scaleProperty;
		protected Property<LinearTransform> transformProperty;

        public ScaleTransformBuilderEntity(FrostySdk.Ebx.ScaleTransformBuilderEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			scaleProperty = new Property<Vec3>(this, Property_Scale, Data.Scale);
			transformProperty = new Property<LinearTransform>(this, Property_Transform, new LinearTransform());
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == scaleProperty.NameHash)
			{
				Matrix m = Matrix.Scaling(scaleProperty.Value.x, scaleProperty.Value.y, scaleProperty.Value.z);
				transformProperty.Value = MakeLinearTransform(m);

				return;
			}

            base.OnPropertyChanged(propertyHash);
        }
    }
}


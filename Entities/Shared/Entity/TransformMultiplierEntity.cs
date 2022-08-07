using System.Collections.Generic;
using LinearTransform = FrostySdk.Ebx.LinearTransform;
using SharpDX;
using Frosty.Core.Viewport;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TransformMultiplierEntityData))]
	public class TransformMultiplierEntity : LogicEntity, IEntityData<FrostySdk.Ebx.TransformMultiplierEntityData>
	{
		protected readonly int Property_In1 = Frosty.Hash.Fnv1.HashString("In1");
		protected readonly int Property_In2 = Frosty.Hash.Fnv1.HashString("In2");
		protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.TransformMultiplierEntityData Data => data as FrostySdk.Ebx.TransformMultiplierEntityData;
		public override string DisplayName => "TransformMultiplier";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In1", Direction.In),
				new ConnectionDesc("In2", Direction.In),
				new ConnectionDesc("Out", Direction.Out)
			};
		}

		protected Property<LinearTransform> inProperty1;
		protected Property<LinearTransform> inProperty2;
		protected Property<LinearTransform> outProperty;

		public TransformMultiplierEntity(FrostySdk.Ebx.TransformMultiplierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			inProperty1 = new Property<LinearTransform>(this, Property_In1, Data.In1);
			inProperty2 = new Property<LinearTransform>(this, Property_In2, Data.In2);
			outProperty = new Property<LinearTransform>(this, Property_Out, new LinearTransform());
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == inProperty1.NameHash || propertyHash == inProperty2.NameHash)
			{
				Matrix m1 = SharpDXUtils.FromLinearTransform(inProperty1.Value);
				Matrix m2 = SharpDXUtils.FromLinearTransform(inProperty2.Value);
				outProperty.Value = MakeLinearTransform(m1 * m2);
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }
    }
}


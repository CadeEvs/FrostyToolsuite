using System.Collections.Generic;
using Vec4 = FrostySdk.Ebx.Vec4;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec4SplitterEntityData))]
	public class Vec4SplitterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.Vec4SplitterEntityData>
	{
		protected readonly int Property_In = Frosty.Hash.Fnv1.HashString("In");
		protected readonly int Property_X = Frosty.Hash.Fnv1.HashString("X");
		protected readonly int Property_Y = Frosty.Hash.Fnv1.HashString("Y");
		protected readonly int Property_Z = Frosty.Hash.Fnv1.HashString("Z");
		protected readonly int Property_W = Frosty.Hash.Fnv1.HashString("W");

		public new FrostySdk.Ebx.Vec4SplitterEntityData Data => data as FrostySdk.Ebx.Vec4SplitterEntityData;
		public override string DisplayName => "Vec4Splitter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In", Direction.In, typeof(Vec4)),
				new ConnectionDesc("X", Direction.Out, typeof(float)),
				new ConnectionDesc("Y", Direction.Out, typeof(float)),
				new ConnectionDesc("Z", Direction.Out, typeof(float)),
				new ConnectionDesc("W", Direction.Out, typeof(float))
			};
		}

		protected Property<Vec4> inProperty;
		protected Property<float> xProperty;
		protected Property<float> yProperty;
		protected Property<float> zProperty;
		protected Property<float> wProperty;

		public Vec4SplitterEntity(FrostySdk.Ebx.Vec4SplitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			inProperty = new Property<Vec4>(this, Property_In, new Vec4());
			xProperty = new Property<float>(this, Property_X);
			yProperty = new Property<float>(this, Property_Y);
			zProperty = new Property<float>(this, Property_Z);
			wProperty = new Property<float>(this, Property_W);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == inProperty.NameHash)
			{
				Vec4 vecValue = inProperty.Value;
				xProperty.Value = vecValue.x;
				yProperty.Value = vecValue.y;
				zProperty.Value = vecValue.z;
				wProperty.Value = vecValue.w;
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }
    }
}


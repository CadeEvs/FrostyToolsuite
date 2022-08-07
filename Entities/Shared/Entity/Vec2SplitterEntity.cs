using System.Collections.Generic;
using Vec2 = FrostySdk.Ebx.Vec2;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec2SplitterEntityData))]
	public class Vec2SplitterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.Vec2SplitterEntityData>
	{
		protected readonly int Property_In = Frosty.Hash.Fnv1.HashString("In");
		protected readonly int Property_X = Frosty.Hash.Fnv1.HashString("X");
		protected readonly int Property_Y = Frosty.Hash.Fnv1.HashString("Y");

		public new FrostySdk.Ebx.Vec2SplitterEntityData Data => data as FrostySdk.Ebx.Vec2SplitterEntityData;
		public override string DisplayName => "Vec2Splitter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In", Direction.In, typeof(Vec2)),
				new ConnectionDesc("X", Direction.Out, typeof(float)),
				new ConnectionDesc("Y", Direction.Out, typeof(float))
			};
		}

		protected Property<Vec2> inProperty;
		protected Property<float> xProperty;
		protected Property<float> yProperty;

        public Vec2SplitterEntity(FrostySdk.Ebx.Vec2SplitterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			inProperty = new Property<Vec2>(this, Property_In, new Vec2());
			xProperty = new Property<float>(this, Property_X);
			yProperty = new Property<float>(this, Property_Y);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == inProperty.NameHash)
			{
				xProperty.Value = inProperty.Value.x;
				yProperty.Value = inProperty.Value.y;
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }
    }
}


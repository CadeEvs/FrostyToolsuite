using System.Collections.Generic;
using Vec3 = FrostySdk.Ebx.Vec3;
using Vec4 = FrostySdk.Ebx.Vec4;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ColorStateEntityData))]
	public class ColorStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ColorStateEntityData>
	{
		protected readonly int Property_Vec3 = Frosty.Hash.Fnv1.HashString("Vec3");
		protected readonly int Property_Vec4 = Frosty.Hash.Fnv1.HashString("Vec4");

		public new FrostySdk.Ebx.ColorStateEntityData Data => data as FrostySdk.Ebx.ColorStateEntityData;
		public override string DisplayName => "ColorState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Vec3", Direction.Out, typeof(Vec3)),
				new ConnectionDesc("Vec4", Direction.Out, typeof(Vec4))
			};
		}

		protected Property<Vec3> vec3Property;
		protected Property<Vec4> vec4Property;

        public ColorStateEntity(FrostySdk.Ebx.ColorStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			vec3Property = new Property<Vec3>(this, Property_Vec3);
			vec4Property = new Property<Vec4>(this, Property_Vec4);
		}

        public override void BeginSimulation()
        {
            base.BeginSimulation();

			if (vec3Property.IsUnset) vec3Property.Value = Data.RGB;
			if (vec4Property.IsUnset) vec4Property.Value = new Vec4() { x = Data.RGB.x, y = Data.RGB.y, z = Data.RGB.z, w = Data.Alpha };
        }
    }
}


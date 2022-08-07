using System.Collections.Generic;
using Vec3 = FrostySdk.Ebx.Vec3;
using Vec4 = FrostySdk.Ebx.Vec4;
using LinearTransform = FrostySdk.Ebx.LinearTransform;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyGateEntityData))]
	public class PropertyGateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PropertyGateEntityData>
	{
		protected readonly int Event_Open = Frosty.Hash.Fnv1.HashString("Open");
		protected readonly int Event_Close = Frosty.Hash.Fnv1.HashString("Close");
		protected readonly int Property_BoolIn = Frosty.Hash.Fnv1.HashString("BoolIn");
		protected readonly int Property_IntIn = Frosty.Hash.Fnv1.HashString("IntIn");
		protected readonly int Property_FloatIn = Frosty.Hash.Fnv1.HashString("FloatIn");
		protected readonly int Property_Vec3In = Frosty.Hash.Fnv1.HashString("Vec3In");
		protected readonly int Property_Vec4In = Frosty.Hash.Fnv1.HashString("Vec4In");
		protected readonly int Property_TransformIn = Frosty.Hash.Fnv1.HashString("TransformIn");
		protected readonly int Property_BoolOut = Frosty.Hash.Fnv1.HashString("BoolOut");
		protected readonly int Property_IntOut = Frosty.Hash.Fnv1.HashString("IntOut");
		protected readonly int Property_FloatOut = Frosty.Hash.Fnv1.HashString("FloatOut");
		protected readonly int Property_Vec3Out = Frosty.Hash.Fnv1.HashString("Vec3Out");
		protected readonly int Property_Vec4Out = Frosty.Hash.Fnv1.HashString("Vec4Out");
		protected readonly int Property_TransformOut = Frosty.Hash.Fnv1.HashString("TransformOut");

		public new FrostySdk.Ebx.PropertyGateEntityData Data => data as FrostySdk.Ebx.PropertyGateEntityData;
		public override string DisplayName => "PropertyGate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Open", Direction.In),
				new ConnectionDesc("Close", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("BoolIn", Direction.In),
				new ConnectionDesc("IntIn", Direction.In),
				new ConnectionDesc("FloatIn", Direction.In),
				new ConnectionDesc("Vec3In", Direction.In),
				new ConnectionDesc("Vec4In", Direction.In),
				new ConnectionDesc("TransformIn", Direction.In),
				new ConnectionDesc("BoolOut", Direction.Out),
				new ConnectionDesc("IntOut", Direction.Out),
				new ConnectionDesc("FloatOut", Direction.Out),
				new ConnectionDesc("Vec3Out", Direction.Out),
				new ConnectionDesc("Vec4Out", Direction.Out),
				new ConnectionDesc("TransformOut", Direction.Out)
			};
		}

		protected Event<InputEvent> openEvent;
		protected Event<InputEvent> closeEvent;

		protected Property<bool> boolInProperty;
		protected Property<bool> boolOutProperty;
		protected Property<int> intInProperty;
		protected Property<int> intOutProperty;
		protected Property<float> floatInProperty;
		protected Property<float> floatOutProperty;
		protected Property<Vec3> vec3InProperty;
		protected Property<Vec3> vec3OutProperty;
		protected Property<Vec4> vec4InProperty;
		protected Property<Vec4> vec4OutProperty;
		protected Property<LinearTransform> transformInProperty;
		protected Property<LinearTransform> transformOutProperty;

		protected bool gateOpen;

		public PropertyGateEntity(FrostySdk.Ebx.PropertyGateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			openEvent = new Event<InputEvent>(this, Event_Open);
			closeEvent = new Event<InputEvent>(this, Event_Close);

			boolInProperty = new Property<bool>(this, Property_BoolIn, Data.BoolIn);
			boolOutProperty = new Property<bool>(this, Property_BoolOut);
			intInProperty = new Property<int>(this, Property_IntIn, Data.IntIn);
			intOutProperty = new Property<int>(this, Property_IntOut);
			floatInProperty = new Property<float>(this, Property_FloatIn, Data.FloatIn);
			floatOutProperty = new Property<float>(this, Property_FloatOut);
			vec3InProperty = new Property<Vec3>(this, Property_Vec3In, Data.Vec3In);
			vec3OutProperty = new Property<Vec3>(this, Property_Vec3Out);
			vec4InProperty = new Property<Vec4>(this, Property_Vec4In, Data.Vec4In);
			vec4OutProperty = new Property<Vec4>(this, Property_Vec4Out);
			transformInProperty = new Property<LinearTransform>(this, Property_TransformIn, Data.TransformIn);
			transformOutProperty = new Property<LinearTransform>(this, Property_TransformOut);

			gateOpen = Data.Default;
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			if (propertyHash == boolInProperty.NameHash)
			{
				if (gateOpen)
                {
					boolOutProperty.Value = boolInProperty.Value;
					return;
                }
			}

			// @todo: other types

            base.OnPropertyChanged(propertyHash);
        }

        public override void OnEvent(int eventHash)
        {
			if (eventHash == openEvent.NameHash)
			{
				gateOpen = true;
				if (Data.WritePropertyOnOpenGate)
				{
					// @todo
				}
				return;
			}
			else if (eventHash == closeEvent.NameHash)
			{
				gateOpen = false;
				return;
			}

            base.OnEvent(eventHash);
        }
    }
}


using System;
using System.Collections.Generic;
using MathOpCode = FrostySdk.Ebx.MathOpCode;
using MathEntityType = FrostySdk.Ebx.MathEntityType;
using LinearTransform = FrostySdk.Ebx.LinearTransform;
using Vec2 = FrostySdk.Ebx.Vec2;
using Vec3 = FrostySdk.Ebx.Vec3;
using Vec4 = FrostySdk.Ebx.Vec4;
using System.Collections;
using FrostySdk.Ebx;
using FrostySdk;
using Frosty.Core;

namespace LevelEditorPlugin.Entities
{
    public enum MathFunctions
    {
        MaxFloat = 2087696823,
        IfInt = 193418243,
        IfFloat = 193418252,
        IfBool = 193418248,
        Round = 194586087,
        ClampInt = 1543335391,
        IntToFloat = 1358875324,
        FloatToInt = 2087826864,
        Ceil = 2087759878
    }

	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MathEntityData))]
	public class MathEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MathEntityData>
	{
        private class Stack
        {
            private List<IList> values = new List<IList>();

            public Stack()
            {
            }

            public void ClearState()
            {
                foreach (IList list in values)
                {
                    list.Clear();
                }
            }

            public void AddType(Type type)
            {
                values.Add((IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type)));
            }

            public void AddValue<T>(T value, int slot)
            {
                IList list = GetList(value.GetType());

                while (list.Count <= slot)
                    list.Add(Activator.CreateInstance(value.GetType()));

                list[slot] = value;
            }

            public T GetValue<T>(int slot)
            {
                IList list = GetList(typeof(T));
                return (T)list[slot];
            }

            public T GetValue<T>(uint slot)
            {
                return GetValue<T>((int)slot);
            }

            private IList GetList(Type type)
            {
                return values.Find(l => l.GetType().GetGenericArguments()[0] == type);
            }
        }

        protected readonly int Event_In = Frosty.Hash.Fnv1.HashString("In");
        protected readonly int Event_OnCalculate = Frosty.Hash.Fnv1.HashString("OnCalculate");
        protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.MathEntityData Data => data as FrostySdk.Ebx.MathEntityData;
		public override string DisplayName => "Math";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get
            {
                List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
                if (Data.EvaluateOnEvent)
                {
                    outEvents.Add(new ConnectionDesc("In", Direction.In));
                }
#if !NFS16
                outEvents.Add(new ConnectionDesc("OnCalculate", Direction.Out)); 
#endif
                return outEvents;
            }
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get
            {
                List<ConnectionDesc> outProperties = new List<ConnectionDesc>();

                foreach (MathEntityInstruction instruction in Data.Assembly.Instructions)
                {
                    switch (instruction.Code)
                    {
                        case MathOpCode.MathOpCode_InputB: outProperties.Add(new ConnectionDesc() { Name = FrostySdk.Utils.GetString(instruction.Param1), Direction = Direction.In, DataType = typeof(bool) }); break;
                        case MathOpCode.MathOpCode_InputF: outProperties.Add(new ConnectionDesc() { Name = FrostySdk.Utils.GetString(instruction.Param1), Direction = Direction.In, DataType = typeof(float) }); break;
                        case MathOpCode.MathOpCode_InputI: outProperties.Add(new ConnectionDesc() { Name = FrostySdk.Utils.GetString(instruction.Param1), Direction = Direction.In, DataType = typeof(int) }); break;
                        case MathOpCode.MathOpCode_InputT: outProperties.Add(new ConnectionDesc() { Name = FrostySdk.Utils.GetString(instruction.Param1), Direction = Direction.In, DataType = typeof(LinearTransform) }); break;
                        case MathOpCode.MathOpCode_InputV2: outProperties.Add(new ConnectionDesc() { Name = FrostySdk.Utils.GetString(instruction.Param1), Direction = Direction.In, DataType = typeof(Vec2) }); break;
                        case MathOpCode.MathOpCode_InputV3: outProperties.Add(new ConnectionDesc() { Name = FrostySdk.Utils.GetString(instruction.Param1), Direction = Direction.In, DataType = typeof(Vec3) }); break;
                        case MathOpCode.MathOpCode_InputV4: outProperties.Add(new ConnectionDesc() { Name = FrostySdk.Utils.GetString(instruction.Param1), Direction = Direction.In, DataType = typeof(Vec4) }); break;
                        case MathOpCode.MathOpCode_Return:
                            {
                                switch ((MathEntityType)instruction.Param1)
                                {
                                    case MathEntityType.MathEntityType_Bool:      outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(bool))); break;
                                    case MathEntityType.MathEntityType_Int:       outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(int))); break;
                                    case MathEntityType.MathEntityType_Float:     outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(float))); break;
                                    case MathEntityType.MathEntityType_Vec2:      outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(Vec2))); break;
                                    case MathEntityType.MathEntityType_Vec3:      outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(Vec3))); break;
                                    case MathEntityType.MathEntityType_Vec4:      outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(Vec4))); break;
                                    case MathEntityType.MathEntityType_Transform: outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(LinearTransform))); break;
                                }
                            }
                            break;
                    }
                }

                return outProperties;
            }
        }
        // we only want the header expression to be rebuilt when it absolutely has to, so that's handled outside of the getter
        public override IEnumerable<string> HeaderRows => m_header;

        protected List<IProperty> paramProperties = new List<IProperty>();
        protected IProperty outProperty;
        protected Event<InputEvent> inEvent;
        protected Event<OutputEvent> onCalculateEvent;

        private Stack stack = new Stack();
        private List<string> m_header = new List<string>();

        public MathEntity(FrostySdk.Ebx.MathEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
            BuildExpressionString();
            foreach (MathEntityInstruction instruction in Data.Assembly.Instructions)
            {
                switch (instruction.Code)
                {
                    case MathOpCode.MathOpCode_InputB: paramProperties.Add(new Property<bool>(this, instruction.Param1)); break;
                    case MathOpCode.MathOpCode_InputF: paramProperties.Add(new Property<float>(this, instruction.Param1)); break;
                    case MathOpCode.MathOpCode_InputI: paramProperties.Add(new Property<int>(this, instruction.Param1)); break;
                    case MathOpCode.MathOpCode_InputT: paramProperties.Add(new Property<LinearTransform>(this, instruction.Param1)); break;
                    case MathOpCode.MathOpCode_InputV2: paramProperties.Add(new Property<Vec2>(this, instruction.Param1)); break;
                    case MathOpCode.MathOpCode_InputV3: paramProperties.Add(new Property<Vec3>(this, instruction.Param1)); break;
                    case MathOpCode.MathOpCode_InputV4: paramProperties.Add(new Property<Vec4>(this, instruction.Param1)); break;
                    case MathOpCode.MathOpCode_Return:
                    {
                            switch((MathEntityType)instruction.Param1)
                            {
                                case MathEntityType.MathEntityType_Bool:      outProperty = new Property<bool>(this, Property_Out); break;
                                case MathEntityType.MathEntityType_Int:       outProperty = new Property<int>(this, Property_Out); break;
                                case MathEntityType.MathEntityType_Float:     outProperty = new Property<float>(this, Property_Out); break;
                                case MathEntityType.MathEntityType_Vec2:      outProperty = new Property<Vec2>(this, Property_Out); break;
                                case MathEntityType.MathEntityType_Vec3:      outProperty = new Property<Vec3>(this, Property_Out); break;
                                case MathEntityType.MathEntityType_Vec4:      outProperty = new Property<Vec4>(this, Property_Out); break;
                                case MathEntityType.MathEntityType_Transform: outProperty = new Property<LinearTransform>(this, Property_Out); break;
                            }
                    }
                    break;
                }
            }

            inEvent = new Event<InputEvent>(this, Event_In);
            onCalculateEvent = new Event<OutputEvent>(this, Event_OnCalculate);

            stack.AddType(typeof(bool));
            stack.AddType(typeof(int));
            stack.AddType(typeof(float));
            stack.AddType(typeof(LinearTransform));
            stack.AddType(typeof(Vec2));
            stack.AddType(typeof(Vec3));
            stack.AddType(typeof(Vec4));
        }

        public override void OnPropertyChanged(int propertyHash)
        {
            IProperty property = paramProperties.Find(p => p.NameHash == propertyHash);
            if (property != null && !Data.EvaluateOnEvent)
            {
                Recalculate();
                return;
            }

            base.OnPropertyChanged(propertyHash);
        }

        public override void OnEvent(int eventHash)
        {
            if (eventHash == inEvent.NameHash && Data.EvaluateOnEvent)
            {
                Recalculate();
                return;
            }

            base.OnEvent(eventHash);
        }

        public override void OnDataModified()
        {
            BuildExpressionString();
            base.OnDataModified();
        }

        private void Recalculate()
        {
            stack.ClearState();
            foreach (MathEntityInstruction instruction in Data.Assembly.Instructions)
            {
                switch (instruction.Code)
                {
                    // params
                    case MathOpCode.MathOpCode_InputB: stack.AddValue(GetParameter<bool>(instruction.Param1), instruction.Result); break;
                    case MathOpCode.MathOpCode_InputF: stack.AddValue(GetParameter<float>(instruction.Param1), instruction.Result); break;
                    case MathOpCode.MathOpCode_InputI: stack.AddValue(GetParameter<int>(instruction.Param1), instruction.Result); break;
                    case MathOpCode.MathOpCode_InputT: stack.AddValue(GetParameter<LinearTransform>(instruction.Param1), instruction.Result); break;
                    case MathOpCode.MathOpCode_InputV2: stack.AddValue(GetParameter<Vec2>(instruction.Param1), instruction.Result); break;
                    case MathOpCode.MathOpCode_InputV3: stack.AddValue(GetParameter<Vec3>(instruction.Param1), instruction.Result); break;
                    case MathOpCode.MathOpCode_InputV4: stack.AddValue(GetParameter<Vec4>(instruction.Param1), instruction.Result); break;

                    // constants
                    case MathOpCode.MathOpCode_ConstF: stack.AddValue(GetConstant<float>(instruction.Param1), instruction.Result); break;
                    case MathOpCode.MathOpCode_ConstB: stack.AddValue(GetConstant<bool>(instruction.Param1), instruction.Result); break;
                    case MathOpCode.MathOpCode_ConstI: stack.AddValue(GetConstant<int>(instruction.Param1), instruction.Result); break;

                    // math - float
                    case MathOpCode.MathOpCode_AddF: stack.AddValue(stack.GetValue<float>(instruction.Param1) + stack.GetValue<float>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_SubF: stack.AddValue(stack.GetValue<float>(instruction.Param1) - stack.GetValue<float>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_PowF: stack.AddValue((float)Math.Pow(stack.GetValue<float>(instruction.Param1), stack.GetValue<float>(instruction.Param2)), instruction.Result); break;
                    case MathOpCode.MathOpCode_NegF: stack.AddValue(-stack.GetValue<float>(instruction.Param1), instruction.Result); break;
                    case MathOpCode.MathOpCode_MulF: stack.AddValue(stack.GetValue<float>(instruction.Param1) * stack.GetValue<float>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_DivF: stack.AddValue(stack.GetValue<float>(instruction.Param1) * stack.GetValue<float>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_LessF: stack.AddValue(stack.GetValue<float>(instruction.Param1) < stack.GetValue<float>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_LessEqF: stack.AddValue(stack.GetValue<float>(instruction.Param1) <= stack.GetValue<float>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_GreaterF: stack.AddValue(stack.GetValue<float>(instruction.Param1) > stack.GetValue<float>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_GreaterEqF: stack.AddValue(stack.GetValue<float>(instruction.Param1) >= stack.GetValue<float>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_EqF: stack.AddValue(stack.GetValue<float>(instruction.Param1) == stack.GetValue<float>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_NotEqF: stack.AddValue(stack.GetValue<float>(instruction.Param1) != stack.GetValue<float>(instruction.Param2), instruction.Result); break;

                    // math - bool
                    case MathOpCode.MathOpCode_NotB: stack.AddValue(!stack.GetValue<bool>(instruction.Param1), instruction.Result); break;
                    case MathOpCode.MathOpCode_AndB: stack.AddValue(stack.GetValue<bool>(instruction.Param1) & stack.GetValue<bool>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_OrB: stack.AddValue(stack.GetValue<bool>(instruction.Param1) | stack.GetValue<bool>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_EqB: stack.AddValue(stack.GetValue<bool>(instruction.Param1) == stack.GetValue<bool>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_NotEqB: stack.AddValue(stack.GetValue<bool>(instruction.Param1) != stack.GetValue<bool>(instruction.Param2), instruction.Result); break;

                    // math - int
                    case MathOpCode.MathOpCode_GreaterI: stack.AddValue(stack.GetValue<int>(instruction.Param1) > stack.GetValue<int>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_GreaterEqI: stack.AddValue(stack.GetValue<int>(instruction.Param1) >= stack.GetValue<int>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_LessI: stack.AddValue(stack.GetValue<int>(instruction.Param1) < stack.GetValue<int>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_LessEqI: stack.AddValue(stack.GetValue<int>(instruction.Param1) <= stack.GetValue<int>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_EqI: stack.AddValue(stack.GetValue<int>(instruction.Param1) == stack.GetValue<int>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_NotEqI: stack.AddValue(stack.GetValue<int>(instruction.Param1) != stack.GetValue<int>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_AddI: stack.AddValue(stack.GetValue<int>(instruction.Param1) + stack.GetValue<int>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_SubI: stack.AddValue(stack.GetValue<int>(instruction.Param1) - stack.GetValue<int>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_MulI: stack.AddValue(stack.GetValue<int>(instruction.Param1) * stack.GetValue<int>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_ModI: stack.AddValue(stack.GetValue<int>(instruction.Param1) % stack.GetValue<int>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_DivI: stack.AddValue(stack.GetValue<int>(instruction.Param1) / stack.GetValue<int>(instruction.Param2), instruction.Result); break;
                    case MathOpCode.MathOpCode_PowI: stack.AddValue((int)Math.Pow(stack.GetValue<int>(instruction.Param1), stack.GetValue<int>(instruction.Param2)), instruction.Result); break;
                    case MathOpCode.MathOpCode_NegI: stack.AddValue(-stack.GetValue<int>(instruction.Param1), instruction.Result); break;

                    // math - Vec2
                    case MathOpCode.MathOpCode_AddV2: stack.AddValue(AddV2(stack.GetValue<Vec2>(instruction.Param1), stack.GetValue<Vec2>(instruction.Param2)), instruction.Result); break;
                    case MathOpCode.MathOpCode_DivV2F: stack.AddValue(DivV2F(stack.GetValue<Vec2>(instruction.Param1), stack.GetValue<float>(instruction.Param2)), instruction.Result); break;
                    case MathOpCode.MathOpCode_DivV2I: stack.AddValue(DivV2I(stack.GetValue<Vec2>(instruction.Param1), stack.GetValue<int>(instruction.Param2)), instruction.Result); break;
                    case MathOpCode.MathOpCode_MulV2F: stack.AddValue(MulV2F(stack.GetValue<Vec2>(instruction.Param1), stack.GetValue<float>(instruction.Param2)), instruction.Result); break;
                    case MathOpCode.MathOpCode_MulV2I: stack.AddValue(MulV2I(stack.GetValue<Vec2>(instruction.Param1), stack.GetValue<int>(instruction.Param2)), instruction.Result); break;
                    case MathOpCode.MathOpCode_NegV2: stack.AddValue(NegV2(stack.GetValue<Vec2>(instruction.Param1)), instruction.Result); break;
                    case MathOpCode.MathOpCode_SubV2: stack.AddValue(SubV2(stack.GetValue<Vec2>(instruction.Param1), stack.GetValue<Vec2>(instruction.Param2)), instruction.Result); break;

                    // functions
                    case MathOpCode.MathOpCode_Func: stack.AddValue(RunFunction(instruction.Param1, Data.Assembly.FunctionCalls[instruction.Param2]), instruction.Result); break;
                    
                    // return
                    case MathOpCode.MathOpCode_Return:
                        {
                            switch (instruction.Param1)
                            {
                                case 1: outProperty.Value = stack.GetValue<bool>(instruction.Result); break;
                                case 2: outProperty.Value = stack.GetValue<int>(instruction.Result); break;
                                case 4: outProperty.Value = stack.GetValue<float>(instruction.Result); break;
                            }
                        }
                        break;

                    default: throw new NotImplementedException($"OpCode {instruction.Code} has not been implemented yet.");
                }
            }
        }

        private void BuildExpressionString()
        {
            // get rid of the previous expression
            m_header.Clear();

            // each type is assigned 32 registers (as of NFS15, may differ for other games)
            // since nothing is being calculated here, all the registers will just contain strings to make it easier to build the final expression
            string[] transformReg = new string[32];
            string[] vec4Reg = new string[32];
            string[] vec3Reg = new string[32];
            string[] vec2Reg = new string[32];
            string[] intReg = new string[32];
            string[] floatReg = new string[32];
            string[] boolReg = new string[32];

            foreach (MathEntityInstruction inst in Data.Assembly.Instructions)
            {
                switch (inst.Code)
                {
                    // constants
                    case MathOpCode.MathOpCode_ConstB: boolReg[inst.Result] = $"{GetConstant<bool>(inst.Param1)}"; break;
                    case MathOpCode.MathOpCode_ConstI: intReg[inst.Result] = $"{GetConstant<int>(inst.Param1)}"; break;
                    case MathOpCode.MathOpCode_ConstF: floatReg[inst.Result] = $"{GetConstant<float>(inst.Param1)}"; break;

                    // params
                    case MathOpCode.MathOpCode_InputB: boolReg[inst.Result] = Utils.GetString(inst.Param1); break;
                    case MathOpCode.MathOpCode_InputI: intReg[inst.Result] = Utils.GetString(inst.Param1); break;
                    case MathOpCode.MathOpCode_InputF: floatReg[inst.Result] = Utils.GetString(inst.Param1); break;
                    case MathOpCode.MathOpCode_InputV2: vec2Reg[inst.Result] = Utils.GetString(inst.Param1); break;
                    case MathOpCode.MathOpCode_InputV3: vec3Reg[inst.Result] = Utils.GetString(inst.Param1); break;
                    case MathOpCode.MathOpCode_InputV4: vec4Reg[inst.Result] = Utils.GetString(inst.Param1); break;
                    case MathOpCode.MathOpCode_InputT: transformReg[inst.Result] = Utils.GetString(inst.Param1); break;

                    // logical operators
                    case MathOpCode.MathOpCode_OrB: boolReg[inst.Result] = $"{boolReg[inst.Param1]} || {boolReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_AndB: boolReg[inst.Result] = $"{boolReg[inst.Param1]} && {boolReg[inst.Param2]}"; break;

                    // comparisons
                    case MathOpCode.MathOpCode_GreaterI: boolReg[inst.Result] = $"{intReg[inst.Param1]} > {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_GreaterF: boolReg[inst.Result] = $"{floatReg[inst.Param1]} > {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_GreaterEqI: boolReg[inst.Result] = $"{intReg[inst.Param1]} >= {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_GreaterEqF: boolReg[inst.Result] = $"{floatReg[inst.Param1]} >= {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_LessI: boolReg[inst.Result] = $"{intReg[inst.Param1]} < {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_LessF: boolReg[inst.Result] = $"{floatReg[inst.Param1]} < {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_LessEqI: boolReg[inst.Result] = $"{intReg[inst.Param1]} <= {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_LessEqF: boolReg[inst.Result] = $"{floatReg[inst.Param1]} <= {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_NotEqI: boolReg[inst.Result] = $"{intReg[inst.Param1]} != {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_NotEqF: boolReg[inst.Result] = $"{floatReg[inst.Param1]} != {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_NotEqB: boolReg[inst.Result] = $"{boolReg[inst.Param1]} + {boolReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_EqI: boolReg[inst.Result] = $"{intReg[inst.Param1]} == {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_EqF: boolReg[inst.Result] = $"{floatReg[inst.Param1]} == {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_EqB: boolReg[inst.Result] = $"{boolReg[inst.Param1]} == {boolReg[inst.Param2]}"; break;

                    // addition
                    case MathOpCode.MathOpCode_AddI: intReg[inst.Result] = $"{intReg[inst.Param1]} + {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_AddF: floatReg[inst.Result] = $"{floatReg[inst.Param1]} + {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_AddV2: vec2Reg[inst.Result] = $"{vec2Reg[inst.Param1]} + {vec2Reg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_AddV3: vec3Reg[inst.Result] = $"{vec3Reg[inst.Param1]} + {vec3Reg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_AddV4: vec4Reg[inst.Result] = $"{vec4Reg[inst.Param1]} + {vec4Reg[inst.Param2]}"; break;

                    // subtraction
                    case MathOpCode.MathOpCode_SubI: intReg[inst.Result] = $"{intReg[inst.Param1]} - {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_SubF: floatReg[inst.Result] = $"{floatReg[inst.Param1]} - {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_SubV2: vec2Reg[inst.Result] = $"{vec2Reg[inst.Param1]} - {vec2Reg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_SubV3: vec3Reg[inst.Result] = $"{vec3Reg[inst.Param1]} - {vec3Reg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_SubV4: vec4Reg[inst.Result] = $"{vec4Reg[inst.Param1]} - {vec4Reg[inst.Param2]}"; break;

                    // multiplication
                    case MathOpCode.MathOpCode_MulF: floatReg[inst.Result] = $"{floatReg[inst.Param1]} * {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_MulI: intReg[inst.Result] = $"{intReg[inst.Param1]} * {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_MulV2F: vec2Reg[inst.Result] = $"{vec2Reg[inst.Param1]} * {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_MulV3F: vec3Reg[inst.Result] = $"{vec3Reg[inst.Param1]} * {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_MulV4F: vec4Reg[inst.Result] = $"{vec4Reg[inst.Param1]} * {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_MulV2I: vec2Reg[inst.Result] = $"{vec2Reg[inst.Param1]} * {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_MulV3I: vec3Reg[inst.Result] = $"{vec3Reg[inst.Param1]} * {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_MulV4I: vec4Reg[inst.Result] = $"{vec4Reg[inst.Param1]} * {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_MulT: transformReg[inst.Result] = $"{transformReg[inst.Param1]} * {transformReg[inst.Param2]}"; break;

                    // division
                    case MathOpCode.MathOpCode_DivI: intReg[inst.Result] = $"{intReg[inst.Param1]} / {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_DivF: floatReg[inst.Result] = $"{floatReg[inst.Param1]} / {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_DivV2F: vec2Reg[inst.Result] = $"{vec2Reg[inst.Param1]} / {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_DivV3F: vec3Reg[inst.Result] = $"{vec3Reg[inst.Param1]} / {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_DivV4F: vec4Reg[inst.Result] = $"{vec4Reg[inst.Param1]} / {floatReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_DivV2I: vec2Reg[inst.Result] = $"{vec2Reg[inst.Param1]} / {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_DivV3I: vec3Reg[inst.Result] = $"{vec3Reg[inst.Param1]} / {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_DivV4I: vec4Reg[inst.Result] = $"{vec4Reg[inst.Param1]} / {intReg[inst.Param2]}"; break;

                    // modulo
                    case MathOpCode.MathOpCode_ModI: intReg[inst.Result] = $"{intReg[inst.Param1]} % {intReg[inst.Param2]}"; break;

                    // negation
                    case MathOpCode.MathOpCode_NegI: intReg[inst.Result] = $"-{intReg[inst.Param1]}"; break;
                    case MathOpCode.MathOpCode_NegF: floatReg[inst.Result] = $"-{floatReg[inst.Param1]}"; break;
                    case MathOpCode.MathOpCode_NegV2: vec2Reg[inst.Result] = $"-{vec2Reg[inst.Param1]}"; break;
                    case MathOpCode.MathOpCode_NegV3: vec3Reg[inst.Result] = $"-{vec3Reg[inst.Param1]}"; break;
                    case MathOpCode.MathOpCode_NegV4: vec4Reg[inst.Result] = $"-{vec4Reg[inst.Param1]}"; break;
                    case MathOpCode.MathOpCode_NotB: boolReg[inst.Result] = $"!{boolReg[inst.Param1]}"; break;

                    // exponent
                    case MathOpCode.MathOpCode_PowI: intReg[inst.Result] = $"{intReg[inst.Param1]} ^ {intReg[inst.Param2]}"; break;
                    case MathOpCode.MathOpCode_PowF: floatReg[inst.Result] = $"{floatReg[inst.Param1]} ^ {floatReg[inst.Param2]}"; break;

                    // field accessors
                    case MathOpCode.MathOpCode_FieldV2:
                        {
                            switch (inst.Param2)
                            {
                                case 0: floatReg[inst.Result] = $"{vec2Reg[inst.Param1]}.x"; break;
                                case 1: floatReg[inst.Result] = $"{vec2Reg[inst.Param1]}.y"; break;
                            }
                            break;
                        }
                    case MathOpCode.MathOpCode_FieldV3:
                        {
                            switch (inst.Param2)
                            {
                                case 0: floatReg[inst.Result] = $"{vec3Reg[inst.Param1]}.x"; break;
                                case 1: floatReg[inst.Result] = $"{vec3Reg[inst.Param1]}.y"; break;
                                case 2: floatReg[inst.Result] = $"{vec3Reg[inst.Param1]}.z"; break;
                            }
                            break;
                        }
                    case MathOpCode.MathOpCode_FieldV4:
                        {
                            switch (inst.Param2)
                            {
                                case 0: floatReg[inst.Result] = $"{vec4Reg[inst.Param1]}.x"; break;
                                case 1: floatReg[inst.Result] = $"{vec4Reg[inst.Param1]}.y"; break;
                                case 2: floatReg[inst.Result] = $"{vec4Reg[inst.Param1]}.z"; break;
                                case 3: floatReg[inst.Result] = $"{vec4Reg[inst.Param1]}.w"; break;
                            }
                            break;
                        }
                    case MathOpCode.MathOpCode_FieldT:
                        {
                            switch (inst.Param2)
                            {
                                case 0: vec3Reg[inst.Result] = $"{transformReg[inst.Param1]}.left"; break;
                                case 1: vec3Reg[inst.Result] = $"{transformReg[inst.Param1]}.up"; break;
                                case 2: vec3Reg[inst.Result] = $"{transformReg[inst.Param1]}.forward"; break;
                                case 3: vec3Reg[inst.Result] = $"{transformReg[inst.Param1]}.trans"; break;
                            }
                            break;
                        }

                    // functions
                    // these are all the functions available as of NFS15, later games probably have more
                    case MathOpCode.MathOpCode_Func:
                        {
                            List<uint> callParams = Data.Assembly.FunctionCalls[inst.Param2].Parameters;
                            switch ((uint)inst.Param1)
                            {
                                case 0x7C6D8553: /* absf */ floatReg[inst.Result] = $"absf({floatReg[callParams[0]]})"; break;
                                case 0x7C6D855C: /* absi */ intReg[inst.Result] = $"absi({intReg[callParams[0]]})"; break;
                                case 0x7C6F98E6: /* mini */ intReg[inst.Result] = $"mini({intReg[callParams[0]]}, {intReg[callParams[1]]})"; break;
                                case 0x7C6F98E9: /* minf */ floatReg[inst.Result] = $"minf({floatReg[callParams[0]]}, {floatReg[callParams[1]]})"; break;
                                case 0x7C6FB9B8: /* maxi */ intReg[inst.Result] = $"maxi({intReg[callParams[0]]}, {intReg[callParams[1]]})"; break;
                                case 0x7C6FB9B7: /* maxf */ floatReg[inst.Result] = $"maxf({floatReg[callParams[0]]}, {floatReg[callParams[1]]})"; break;
                                case 0x0B874C7A: /* cos */ floatReg[inst.Result] = $"cos({floatReg[callParams[0]]})"; break;
                                case 0x0B8790B1: /* sin */ floatReg[inst.Result] = $"sin({floatReg[callParams[0]]})"; break;
                                case 0x0B8761FE: /* tan */ floatReg[inst.Result] = $"tan({floatReg[callParams[0]]})"; break;
                                case 0x7C6DA01B: /* acos */ floatReg[inst.Result] = $"acos({floatReg[callParams[0]]})"; break;
                                case 0x7C6DE550: /* asin */ floatReg[inst.Result] = $"asin({floatReg[callParams[0]]})"; break;
                                case 0x7C6DBE1F: /* atan */ floatReg[inst.Result] = $"atan({floatReg[callParams[0]]})"; break;
                                case 0x0BA1C827: /* sqrtf */ floatReg[inst.Result] = $"sqrtf({floatReg[callParams[0]]})"; break;
                                case 0x0BA1C828: /* sqrti */ intReg[inst.Result] = $"sqrti({intReg[callParams[0]]})"; break;
                                case 0x0A595A68: /* lerpf */ floatReg[inst.Result] = $"lerpf({floatReg[callParams[0]]}, {floatReg[callParams[1]]}, {floatReg[callParams[2]]})"; break;
                                case 0x5BFD6DD0: /* clampf */ floatReg[inst.Result] = $"clampf({floatReg[callParams[0]]}, {floatReg[callParams[1]]}, {floatReg[callParams[2]]})"; break;
                                case 0x5BFD6DDF: /* clampi */ intReg[inst.Result] = $"clampi({intReg[callParams[0]]}, {intReg[callParams[1]]}, {intReg[callParams[2]]})"; break;
                                case 0x50FECABC: /* floati */ floatReg[inst.Result] = $"float({intReg[callParams[0]]})"; break;
                                case 0x50FECAB7: /* floatb */ floatReg[inst.Result] = $"float({boolReg[callParams[0]]})"; break;
                                case 0x7C71B5B0: /* intf */ intReg[inst.Result] = $"int({floatReg[callParams[0]]})"; break;
                                case 0x7C71B5B4: /* intb */ intReg[inst.Result] = $"int({boolReg[callParams[0]]})"; break;
                                case 0x0B9925E7: /* round */ floatReg[inst.Result] = $"round({floatReg[callParams[0]]})"; break;
                                case 0x7C70B006: /* ceil */ floatReg[inst.Result] = $"ceil({floatReg[callParams[0]]})"; break;
                                case 0x0A36467D: /* floor */ floatReg[inst.Result] = $"floor({floatReg[callParams[0]]})"; break;
                                case 0x7C76EA27: /* vec2 */ vec2Reg[inst.Result] = $"vec2({floatReg[callParams[0]]}, {floatReg[callParams[1]]})"; break;
                                case 0x7C76EA26: /* vec3 */ vec3Reg[inst.Result] = $"vec3({floatReg[callParams[0]]}, {floatReg[callParams[1]]}, {floatReg[callParams[2]]})"; break;
                                case 0x7C76EA21: /* vec4 */ vec4Reg[inst.Result] = $"vec4({floatReg[callParams[0]]}, {floatReg[callParams[1]]}, {floatReg[callParams[2]]}, {floatReg[callParams[3]]})"; break;
                                case 0x09CBC7BE: /* dotv2 */ floatReg[inst.Result] = $"dotv2({vec2Reg[callParams[0]]}, {vec2Reg[callParams[1]]})"; break;
                                case 0x09CBC7BF: /* dotv3 */ floatReg[inst.Result] = $"dotv3({vec3Reg[callParams[0]]}, {vec3Reg[callParams[1]]})"; break;
                                case 0x09CBC7B8: /* dotv4 */ floatReg[inst.Result] = $"dotv4({vec4Reg[callParams[0]]}, {vec4Reg[callParams[1]]})"; break;
                                case 0x0A8EF17B: /* cross */ vec3Reg[inst.Result] = $"cross({vec3Reg[callParams[0]]}, {vec3Reg[callParams[1]]})"; break;
                                case 0x623E329F: /* normv2 */ floatReg[inst.Result] = $"normv2({vec2Reg[callParams[0]]})"; break;
                                case 0x623E329E: /* normv3 */ floatReg[inst.Result] = $"normv3({vec3Reg[callParams[0]]})"; break;
                                case 0x623E3299: /* normv4 */ floatReg[inst.Result] = $"normv4({vec4Reg[callParams[0]]})"; break;
                                case 0x5584A94A: /* lerpv2 */ vec2Reg[inst.Result] = $"lerpv2({vec2Reg[callParams[0]]}, {vec2Reg[callParams[1]]}, {floatReg[callParams[2]]})"; break;
                                case 0x5584A94B: /* lerpv3 */ vec3Reg[inst.Result] = $"lerpv3({vec3Reg[callParams[0]]}, {vec3Reg[callParams[1]]}, {floatReg[callParams[2]]})"; break;
                                case 0x5584A94C: /* lerpv4 */ vec4Reg[inst.Result] = $"lerpv4({vec4Reg[callParams[0]]}, {vec4Reg[callParams[1]]}, {floatReg[callParams[2]]})"; break;
                                case 0x0BAD259D: /* slerp */ vec4Reg[inst.Result] = $"slerp({vec3Reg[callParams[0]]}, {vec3Reg[callParams[1]]}, {floatReg[callParams[2]]})"; break;
                                case 0xECF435E2: /* distancev2 */ floatReg[inst.Result] = $"distancev2({vec2Reg[callParams[0]]}, {vec2Reg[callParams[1]]})"; break;
                                case 0xECF435E3: /* distancev3 */ floatReg[inst.Result] = $"distancev3({vec3Reg[callParams[0]]}, {vec3Reg[callParams[1]]})"; break;
                                case 0xECF435E4: /* distancev4 */ floatReg[inst.Result] = $"distancev4({vec4Reg[callParams[0]]}, {vec4Reg[callParams[1]]})"; break;
                                case 0xEA9C0AB2: /* normalv2 */ vec2Reg[inst.Result] = $"normalv2({vec2Reg[callParams[0]]})"; break;
                                case 0xEA9C0AB3: /* normalv3 */ vec3Reg[inst.Result] = $"normalv3({vec3Reg[callParams[0]]})"; break;
                                case 0xEA9C0AB4: /* normalv4 */ vec4Reg[inst.Result] = $"normalv4({vec4Reg[callParams[0]]})"; break;
                                case 0x30E0752E: /* translation */ transformReg[inst.Result] = $"translation({vec3Reg[callParams[0]]})"; break;
                                case 0x32FB2E69: /* rotationx */ transformReg[inst.Result] = $"rotationx({floatReg[callParams[0]]})"; break;
                                case 0x32FB2E68: /* rotationy */ transformReg[inst.Result] = $"rotationy({floatReg[callParams[0]]})"; break;
                                case 0x32FB2E6B: /* rotationz */ transformReg[inst.Result] = $"rotationz({floatReg[callParams[0]]})"; break;
                                case 0x0BAB517D: /* scale */
                                    // going off of NFS15, it can only use either 1 param or 3, there's no in between
                                    // could be different for other games
                                    if (callParams.Count <= 1) transformReg[inst.Result] = $"scale({floatReg[callParams[0]]})";
                                    else transformReg[inst.Result] = $"scale({floatReg[callParams[0]]}, {floatReg[callParams[1]]}, {floatReg[callParams[2]]})";
                                    break;
                                case 0xFEF06531: /* rotationAndTranslation */ transformReg[inst.Result] = $"rotationAndTranslation({transformReg[callParams[0]]}, {vec3Reg[callParams[1]]})"; break;
                                case 0x6FF5791B: /* lookAtTransform */
                                    // requires at least 2 params
                                    if (callParams.Count <= 2) transformReg[inst.Result] = $"lookAtTransform({vec3Reg[callParams[0]]}, {vec3Reg[callParams[1]]})";
                                    else transformReg[inst.Result] = $"lookAtTransform({vec3Reg[callParams[0]]}, {vec3Reg[callParams[1]]}, {vec3Reg[callParams[2]]})";
                                    break;
                                case 0x569716D5: /* inverse */ transformReg[inst.Result] = $"inverse({transformReg[callParams[0]]})"; break;
                                case 0xC7D3B8C6: /* fullInverse */ transformReg[inst.Result] = $"fullInverse({transformReg[callParams[0]]})"; break;
                                case 0x7EBE7DDC: /* rotate */ vec3Reg[inst.Result] = $"rotate({vec3Reg[callParams[0]]}, {transformReg[callParams[1]]})"; break;
                                case 0xF5E8DCED: /* invRotate */ vec3Reg[inst.Result] = $"invRotate({vec3Reg[callParams[0]]}, {transformReg[callParams[1]]})"; break;
                                case 0x18BB7349: /* transform */ vec3Reg[inst.Result] = $"transform({vec3Reg[callParams[0]]}, {transformReg[callParams[1]]})"; break;
                                case 0x72FCF358: /* invTransform */ vec3Reg[inst.Result] = $"invTransform({vec3Reg[callParams[0]]}, {transformReg[callParams[1]]})"; break;
                                case 0x2AC63C95: /* isWorldSpaceTransform */ boolReg[inst.Result] = $"isWorldSpaceTransform({transformReg[callParams[0]]})"; break;
                                case 0xE5C0729D: /* asWorldSpaceTransform */ transformReg[inst.Result] = $"asWorldSpaceTransform({transformReg[callParams[0]]})"; break;
                                case 0x394FBBF2: /* asLocalSpaceTransform */ transformReg[inst.Result] = $"asLocalSpaceTransform({transformReg[callParams[0]]})"; break;
                                case 0x0B875408: /* ifb */ boolReg[inst.Result] = $"ifb({boolReg[callParams[0]]}, {boolReg[callParams[1]]}, {boolReg[callParams[2]]})"; break;
                                case 0x0B875403: /* ifi */ intReg[inst.Result] = $"ifi({boolReg[callParams[0]]}, {intReg[callParams[1]]}, {intReg[callParams[2]]})"; break;
                                case 0x0B87540C: /* iff */ floatReg[inst.Result] = $"iff({boolReg[callParams[0]]}, {floatReg[callParams[1]]}, {floatReg[callParams[2]]})"; break;
                                case 0x7C71D7AE: /* ifv2 */ vec2Reg[inst.Result] = $"ifv2({boolReg[callParams[0]]}, {vec2Reg[callParams[1]]}, {vec2Reg[callParams[2]]})"; break;
                                case 0x7C71D7AF: /* ifv3 */ vec3Reg[inst.Result] = $"ifv3({boolReg[callParams[0]]}, {vec3Reg[callParams[1]]}, {vec3Reg[callParams[2]]})"; break;
                                case 0x7C71D7A8: /* ifv4 */ vec4Reg[inst.Result] = $"ifv4({boolReg[callParams[0]]}, {vec4Reg[callParams[1]]}, {vec4Reg[callParams[2]]})"; break;
                                case 0x0B87541E: /* ift */ transformReg[inst.Result] = $"ift({boolReg[callParams[0]]}, {transformReg[callParams[1]]}, {transformReg[callParams[2]]})"; break;
                                case 0x7C79F742: /* xorb */ boolReg[inst.Result] = $"xorb({boolReg[callParams[0]]}, {boolReg[callParams[1]]})"; break;

                                default: App.Logger.LogError($"Function {inst.Param1} has not been implemented yet."); return;
                            }
                            break;
                        }

                    // return
                    case MathOpCode.MathOpCode_Return:
                        {
                            switch ((MathEntityType)inst.Param1)
                            {
                                case MathEntityType.MathEntityType_Bool: m_header.Add(boolReg[inst.Result]); return;
                                case MathEntityType.MathEntityType_Int: m_header.Add(intReg[inst.Result]); return;
                                case MathEntityType.MathEntityType_Float: m_header.Add(floatReg[inst.Result]); return;
                                case MathEntityType.MathEntityType_Vec2: m_header.Add(vec2Reg[inst.Result]); return;
                                case MathEntityType.MathEntityType_Vec3: m_header.Add(vec3Reg[inst.Result]); return;
                                case MathEntityType.MathEntityType_Vec4: m_header.Add(vec4Reg[inst.Result]); return;
                                case MathEntityType.MathEntityType_Transform: m_header.Add(transformReg[inst.Result]); return;
                            }
                        }
                        break;

                    default: App.Logger.LogError($"OpCode {inst.Code} has not been implemented yet."); return;
                }
            }
        }

        private T GetParameter<T>(int paramHash)
        {
            IProperty param = paramProperties.Find(p => p.NameHash == paramHash);
            return (T)param.Value;
        }

        private T GetConstant<T>(int constBuffer)
        {
            if (typeof(T) == typeof(bool)) return (T)(object)(constBuffer != 0);
            else if (typeof(T) == typeof(int)) return (T)(object)constBuffer;
            else if (typeof(T) == typeof(float)) return (T)(object)BitConverter.ToSingle(BitConverter.GetBytes(constBuffer), 0);
            return default(T);
        }

        private object RunFunction(int function, FrostySdk.Ebx.MathEntityFunctionCall functionData)
        {
            switch ((MathFunctions)function)
            {
                case MathFunctions.MaxFloat: return Math.Max(stack.GetValue<float>(functionData.Parameters[0]), stack.GetValue<float>(functionData.Parameters[1]));
                case MathFunctions.Round: return (float)Math.Round(stack.GetValue<float>(functionData.Parameters[0]));
                case MathFunctions.IntToFloat: return (float)stack.GetValue<int>(functionData.Parameters[0]);
                case MathFunctions.FloatToInt: return (int)stack.GetValue<float>(functionData.Parameters[0]);
                case MathFunctions.Ceil: return (int)Math.Ceiling(stack.GetValue<float>(functionData.Parameters[0]));
                case MathFunctions.IfInt:
                    {
                        bool condition = stack.GetValue<bool>(functionData.Parameters[0]);
                        return (condition)
                            ? stack.GetValue<int>(functionData.Parameters[1])
                            : stack.GetValue<int>(functionData.Parameters[2]);
                    }
                case MathFunctions.IfFloat:
                    {
                        bool condition = stack.GetValue<bool>(functionData.Parameters[0]);
                        return (condition)
                            ? stack.GetValue<float>(functionData.Parameters[1])
                            : stack.GetValue<float>(functionData.Parameters[2]);
                    }
                case MathFunctions.IfBool:
                    {
                        bool condition = stack.GetValue<bool>(functionData.Parameters[0]);
                        return (condition)
                            ? stack.GetValue<bool>(functionData.Parameters[1])
                            : stack.GetValue<bool>(functionData.Parameters[2]);
                    }
                case MathFunctions.ClampInt:
                    {
                        int value = stack.GetValue<int>(functionData.Parameters[0]);
                        int min = stack.GetValue<int>(functionData.Parameters[1]);
                        int max = stack.GetValue<int>(functionData.Parameters[2]);

                        if (value < min) value = min;
                        if (value > max) value = max;

                        return value;
                    }
            }

            throw new NotImplementedException($"Function {FrostySdk.Utils.GetString(function)} is not implemented yet.");
        }

        private Vec2 AddV2(Vec2 a, Vec2 b)
        {
            return new Vec2() { x = a.x + b.x, y = a.y + b.y };
        }

        private Vec2 SubV2(Vec2 a, Vec2 b)
        {
            return new Vec2() { x = a.x - b.x, y = a.y - b.y };
        }

        private Vec2 DivV2F(Vec2 a, float b)
        {
            return new Vec2() { x = a.x / b, y = a.y / b };
        }

        private Vec2 DivV2I(Vec2 a, int b)
        {
            return new Vec2() { x = a.x / b, y = a.y / b };
        }

        private Vec2 MulV2F(Vec2 a, float b)
        {
            return new Vec2() { x = a.x * b, y = a.y * b };
        }

        private Vec2 MulV2I(Vec2 a, int b)
        {
            return new Vec2() { x = a.x * b, y = a.y * b };
        }

        private Vec2 NegV2(Vec2 a)
        {
            return new Vec2() { x = -a.x, y = -a.y };
        }
    }
}


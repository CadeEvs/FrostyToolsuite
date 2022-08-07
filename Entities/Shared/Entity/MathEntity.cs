using System;
using System.Collections.Generic;
using MathOpCode = FrostySdk.Ebx.MathOpCode;
using LinearTransform = FrostySdk.Ebx.LinearTransform;
using Vec2 = FrostySdk.Ebx.Vec2;
using Vec3 = FrostySdk.Ebx.Vec3;
using Vec4 = FrostySdk.Ebx.Vec4;
using System.Collections;
using FrostySdk.Ebx;

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
                    outEvents.Add(new ConnectionDesc("OnCalculate", Direction.Out));
                }
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
                                switch (instruction.Param1)
                                {
                                    case 1: outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(bool))); break;
                                    case 2: outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(int))); break;
                                    case 4: outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(float))); break;
                                    case 8: outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(Vec2))); break;
                                    case 16: outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(Vec3))); break;
                                    case 32: outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(Vec4))); break;
                                    case 64: outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(LinearTransform))); break;
                                }
                            }
                            break;
                    }
                }

                return outProperties;
            }
        }

        protected List<IProperty> paramProperties = new List<IProperty>();
        protected IProperty outProperty;
        protected Event<InputEvent> inEvent;
        protected Event<OutputEvent> onCalculateEvent;

        private Stack stack = new Stack();

        public MathEntity(FrostySdk.Ebx.MathEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
            SetFlags(EntityFlags.HasLogic);
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
                            switch(instruction.Param1)
                            {
                                case 1: outProperty = new Property<bool>(this, Property_Out); break;
                                case 2: outProperty = new Property<int>(this, Property_Out); break;
                                case 4: outProperty = new Property<float>(this, Property_Out); break;
                                case 8: outProperty = new Property<Vec2>(this, Property_Out); break;
                                case 16: outProperty = new Property<Vec3>(this, Property_Out); break;
                                case 32: outProperty = new Property<Vec4>(this, Property_Out); break;
                                case 64: outProperty = new Property<LinearTransform>(this, Property_Out); break;
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


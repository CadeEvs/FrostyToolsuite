using System;
using System.Collections.Generic;
using MathOp = FrostySdk.Ebx.MathOp;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MathOpEntityData))]
	public class MathOpEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MathOpEntityData>
	{
		protected readonly int Property_Out = Frosty.Hash.Fnv1.HashString("Out");

		public new FrostySdk.Ebx.MathOpEntityData Data => data as FrostySdk.Ebx.MathOpEntityData;
		public override string DisplayName => "MathOp";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				for (int i = 0; i < Data.Operators.Count + 1; i++)
                {
					outProperties.Add(new ConnectionDesc($"In{i}", Direction.In, typeof(float)));
                }
				outProperties.Add(new ConnectionDesc("Out", Direction.Out, typeof(float)));
				return outProperties;
			}
		}

		protected List<Property<float>> inProperties = new List<Property<float>>();
		protected Property<float> outProperty;

		public MathOpEntity(FrostySdk.Ebx.MathOpEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			for (int i = 0; i < Data.Operators.Count + 1; i++)
            {
				int hash = Frosty.Hash.Fnv1.HashString($"In{i}");
				inProperties.Add(new Property<float>(this, hash, 0.0f));
            }
			outProperty = new Property<float>(this, Property_Out);
		}

        public override void OnPropertyChanged(int propertyHash)
        {
			int index = inProperties.FindIndex(p => p.NameHash == propertyHash);
			if (index != -1)
			{
				float retVal = inProperties[0].Value;
				int i = 1;

				foreach (MathOp mathOp in Data.Operators)
				{
					switch (mathOp)
                    {
						case MathOp.MathOp_Add: retVal += inProperties[i++].Value; break;
						case MathOp.MathOp_Divide: retVal /= inProperties[i++].Value; break;
						case MathOp.MathOp_Exponent: retVal = (float)Math.Pow(retVal, inProperties[i++].Value); break;
						case MathOp.MathOp_Max: retVal = Math.Max(retVal, inProperties[i++].Value); break;
						case MathOp.MathOp_Min: retVal = Math.Min(retVal, inProperties[i++].Value); break;
						case MathOp.MathOp_Modulo: retVal %= inProperties[i++].Value; break;
						case MathOp.MathOp_Multiply: retVal *= inProperties[i++].Value; break;
						case MathOp.MathOp_Subtract: retVal -= inProperties[i++].Value; break;
                    }
				}

				outProperty.Value = retVal;
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }
    }
}


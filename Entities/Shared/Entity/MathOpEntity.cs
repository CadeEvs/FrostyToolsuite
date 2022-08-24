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
		public override IEnumerable<string> HeaderRows => m_header;

		protected List<Property<float>> inProperties = new List<Property<float>>();
		protected Property<float> outProperty;
		private List<string> m_header = new List<string>();

		public MathOpEntity(FrostySdk.Ebx.MathOpEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			BuildExpressionString();
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
						case MathOp.MathOp_Add:		 retVal += inProperties[i++].Value; break;
						case MathOp.MathOp_Subtract: retVal -= inProperties[i++].Value; break;
						case MathOp.MathOp_Multiply: retVal *= inProperties[i++].Value; break;
						case MathOp.MathOp_Divide:	 retVal /= inProperties[i++].Value; break;
						case MathOp.MathOp_Min:		 retVal =  Math.Min(retVal, inProperties[i++].Value); break;
						case MathOp.MathOp_Max:		 retVal =  Math.Max(retVal, inProperties[i++].Value); break;
#if NFS16 || NFS_HEAT
						case MathOp.MathOp_Atan2:	 retVal =  (float)Math.Atan2(retVal, inProperties[i++].Value); break; 
#endif
                        case MathOp.MathOp_Modulo:	 retVal %= inProperties[i++].Value; break;
						case MathOp.MathOp_Exponent: retVal =  (float)Math.Pow(retVal, inProperties[i++].Value); break;
                    }
				}

				outProperty.Value = retVal;
				return;
			}

            base.OnPropertyChanged(propertyHash);
        }

        public override void OnDataModified()
        {
			BuildExpressionString();
            base.OnDataModified();
        }

        private void BuildExpressionString()
        {
            // get rid of the previous expression
			m_header.Clear();

			string expr = "In0";
			for (int i = 0; i < Data.Operators.Count; i++)
			{
				switch (Data.Operators[i])
				{
					case MathOp.MathOp_Add: expr += $" + In{i + 1}"; break;
					case MathOp.MathOp_Subtract: expr += $" - In{i + 1}"; break;
					case MathOp.MathOp_Multiply: expr += $" * In{i + 1}"; break;
					case MathOp.MathOp_Divide: expr += $" / In{i + 1}"; break;
					case MathOp.MathOp_Min: expr = $"min({expr}, In{i + 1})"; break;
					case MathOp.MathOp_Max: expr = $"max({expr}, In{i + 1})"; break;
#if NFS16 || NFS_HEAT
					case MathOp.MathOp_Atan2: expr = $"atan2({expr}, In{i + 1})"; break;
#endif
					case MathOp.MathOp_Modulo: expr += $" % In{i + 1}"; break;
					case MathOp.MathOp_Exponent: expr += $" ^ In{i + 1}"; break;
				}
				expr = $"({expr})";
			}
			expr = $"Expr: {expr}";
			m_header.Add(expr);
		}

    }
}


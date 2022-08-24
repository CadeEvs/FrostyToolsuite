using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VectorMathOpEntityData))]
	public class VectorMathOpEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VectorMathOpEntityData>
	{
		public new FrostySdk.Ebx.VectorMathOpEntityData Data => data as FrostySdk.Ebx.VectorMathOpEntityData;
		public override string DisplayName => "VectorMathOp";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				switch (Data.MathOperator)
                {
					case FrostySdk.Ebx.VectorMathOp.VectorMathOp_Add:
					case FrostySdk.Ebx.VectorMathOp.VectorMathOp_Subtract:
					case FrostySdk.Ebx.VectorMathOp.VectorMathOp_Cross:
						outProperties.Add(new ConnectionDesc($"In1_Vec3", Direction.In, typeof(FrostySdk.Ebx.Vec3)));
						outProperties.Add(new ConnectionDesc($"In2_Vec3", Direction.In, typeof(FrostySdk.Ebx.Vec3)));
						outProperties.Add(new ConnectionDesc($"Out_Vec3", Direction.Out, typeof(FrostySdk.Ebx.Vec3)));
						break;
					case FrostySdk.Ebx.VectorMathOp.VectorMathOp_MultiplyByFloat:
					case FrostySdk.Ebx.VectorMathOp.VectorMathOp_DivideByFloat:
						outProperties.Add(new ConnectionDesc($"In1_Vec3", Direction.In, typeof(FrostySdk.Ebx.Vec3)));
						outProperties.Add(new ConnectionDesc($"In2_Float", Direction.In, typeof(float)));
						outProperties.Add(new ConnectionDesc($"Out_Vec3", Direction.Out, typeof(FrostySdk.Ebx.Vec3)));
						break;
					case FrostySdk.Ebx.VectorMathOp.VectorMathOp_Dot:
						outProperties.Add(new ConnectionDesc($"In1_Vec3", Direction.In, typeof(FrostySdk.Ebx.Vec3)));
						outProperties.Add(new ConnectionDesc($"In2_Vec3", Direction.In, typeof(FrostySdk.Ebx.Vec3)));
						outProperties.Add(new ConnectionDesc($"Out_Float", Direction.Out, typeof(float)));
						break;
					case FrostySdk.Ebx.VectorMathOp.VectorMathOp_Length:
						outProperties.Add(new ConnectionDesc($"In1_Vec3", Direction.In, typeof(FrostySdk.Ebx.Vec3)));
						outProperties.Add(new ConnectionDesc($"Length_Float", Direction.Out, typeof(float)));
						outProperties.Add(new ConnectionDesc($"Normal_Vec3", Direction.Out, typeof(FrostySdk.Ebx.Vec3)));
						break;
				}
				return outProperties;
			}
		}
		public override IEnumerable<string> HeaderRows
        {
			get => new List<string>()
			{
				Data.MathOperator.ToString()
			};
        }

        public VectorMathOpEntity(FrostySdk.Ebx.VectorMathOpEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


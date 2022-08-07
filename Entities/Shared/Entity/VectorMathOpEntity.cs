using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.VectorMathOpEntityData))]
	public class VectorMathOpEntity : LogicEntity, IEntityData<FrostySdk.Ebx.VectorMathOpEntityData>
	{
		public new FrostySdk.Ebx.VectorMathOpEntityData Data => data as FrostySdk.Ebx.VectorMathOpEntityData;
		public override string DisplayName => "VectorMathOp";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
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


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MathIntOpEntityData))]
	public class MathIntOpEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MathIntOpEntityData>
	{
		public new FrostySdk.Ebx.MathIntOpEntityData Data => data as FrostySdk.Ebx.MathIntOpEntityData;
		public override string DisplayName => "MathIntOp";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				for (int i = 0; i < Data.Operators.Count + 1; i++)
				{
					outProperties.Add(new ConnectionDesc() { Name = $"In{i}", Direction = Direction.In });
				}
				outProperties.Add(new ConnectionDesc("Out", Direction.Out));
				return outProperties;
			}
		}

        public MathIntOpEntity(FrostySdk.Ebx.MathIntOpEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


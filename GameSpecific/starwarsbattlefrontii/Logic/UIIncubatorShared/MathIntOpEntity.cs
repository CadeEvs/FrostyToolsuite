using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MathIntOpEntityData))]
	public class MathIntOpEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MathIntOpEntityData>
	{
		public new FrostySdk.Ebx.MathIntOpEntityData Data => data as FrostySdk.Ebx.MathIntOpEntityData;
		public override string DisplayName => "MathIntOp";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public MathIntOpEntity(FrostySdk.Ebx.MathIntOpEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


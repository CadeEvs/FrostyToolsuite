using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsActionBaseData))]
	public class PhysicsActionBase : LogicEntity, IEntityData<FrostySdk.Ebx.PhysicsActionBaseData>
	{
		public new FrostySdk.Ebx.PhysicsActionBaseData Data => data as FrostySdk.Ebx.PhysicsActionBaseData;
		public override string DisplayName => "PhysicsActionBase";

		public PhysicsActionBase(FrostySdk.Ebx.PhysicsActionBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


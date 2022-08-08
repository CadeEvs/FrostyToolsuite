using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsDrivenAnimationEntityData))]
	public class PhysicsDrivenAnimationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PhysicsDrivenAnimationEntityData>
	{
		public new FrostySdk.Ebx.PhysicsDrivenAnimationEntityData Data => data as FrostySdk.Ebx.PhysicsDrivenAnimationEntityData;
		public override string DisplayName => "PhysicsDrivenAnimation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PhysicsDrivenAnimationEntity(FrostySdk.Ebx.PhysicsDrivenAnimationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


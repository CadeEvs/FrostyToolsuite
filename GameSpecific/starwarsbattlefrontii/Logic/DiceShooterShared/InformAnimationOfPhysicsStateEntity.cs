using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InformAnimationOfPhysicsStateEntityData))]
	public class InformAnimationOfPhysicsStateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.InformAnimationOfPhysicsStateEntityData>
	{
		public new FrostySdk.Ebx.InformAnimationOfPhysicsStateEntityData Data => data as FrostySdk.Ebx.InformAnimationOfPhysicsStateEntityData;
		public override string DisplayName => "InformAnimationOfPhysicsState";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public InformAnimationOfPhysicsStateEntity(FrostySdk.Ebx.InformAnimationOfPhysicsStateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


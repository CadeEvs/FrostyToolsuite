using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsBodyLauncherEntityData))]
	public class PhysicsBodyLauncherEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PhysicsBodyLauncherEntityData>
	{
		public new FrostySdk.Ebx.PhysicsBodyLauncherEntityData Data => data as FrostySdk.Ebx.PhysicsBodyLauncherEntityData;
		public override string DisplayName => "PhysicsBodyLauncher";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PhysicsBodyLauncherEntity(FrostySdk.Ebx.PhysicsBodyLauncherEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


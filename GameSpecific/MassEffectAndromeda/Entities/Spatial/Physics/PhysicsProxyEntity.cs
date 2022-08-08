using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PhysicsProxyEntityData))]
	public class PhysicsProxyEntity : ComponentEntity, IEntityData<FrostySdk.Ebx.PhysicsProxyEntityData>
	{
		public new FrostySdk.Ebx.PhysicsProxyEntityData Data => data as FrostySdk.Ebx.PhysicsProxyEntityData;
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PhysicsProxyEntity(FrostySdk.Ebx.PhysicsProxyEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


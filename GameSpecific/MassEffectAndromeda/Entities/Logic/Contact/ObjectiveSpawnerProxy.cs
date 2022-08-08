using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObjectiveSpawnerProxyData))]
	public class ObjectiveSpawnerProxy : LogicEntity, IEntityData<FrostySdk.Ebx.ObjectiveSpawnerProxyData>
	{
		public new FrostySdk.Ebx.ObjectiveSpawnerProxyData Data => data as FrostySdk.Ebx.ObjectiveSpawnerProxyData;
		public override string DisplayName => "ObjectiveSpawnerProxy";

		public ObjectiveSpawnerProxy(FrostySdk.Ebx.ObjectiveSpawnerProxyData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


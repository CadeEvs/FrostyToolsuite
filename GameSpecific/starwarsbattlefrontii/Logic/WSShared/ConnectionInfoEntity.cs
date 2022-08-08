using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConnectionInfoEntityData))]
	public class ConnectionInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ConnectionInfoEntityData>
	{
		public new FrostySdk.Ebx.ConnectionInfoEntityData Data => data as FrostySdk.Ebx.ConnectionInfoEntityData;
		public override string DisplayName => "ConnectionInfo";

		public ConnectionInfoEntity(FrostySdk.Ebx.ConnectionInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConnectionStateData))]
	public class ConnectionState : LogicEntity, IEntityData<FrostySdk.Ebx.ConnectionStateData>
	{
		public new FrostySdk.Ebx.ConnectionStateData Data => data as FrostySdk.Ebx.ConnectionStateData;
		public override string DisplayName => "ConnectionState";

		public ConnectionState(FrostySdk.Ebx.ConnectionStateData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


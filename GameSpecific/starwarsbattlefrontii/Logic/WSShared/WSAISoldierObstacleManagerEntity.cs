using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSAISoldierObstacleManagerEntityData))]
	public class WSAISoldierObstacleManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSAISoldierObstacleManagerEntityData>
	{
		public new FrostySdk.Ebx.WSAISoldierObstacleManagerEntityData Data => data as FrostySdk.Ebx.WSAISoldierObstacleManagerEntityData;
		public override string DisplayName => "WSAISoldierObstacleManager";

		public WSAISoldierObstacleManagerEntity(FrostySdk.Ebx.WSAISoldierObstacleManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


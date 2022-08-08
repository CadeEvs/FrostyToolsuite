using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ObstacleControllerEntityData))]
	public class ObstacleControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ObstacleControllerEntityData>
	{
		public new FrostySdk.Ebx.ObstacleControllerEntityData Data => data as FrostySdk.Ebx.ObstacleControllerEntityData;
		public override string DisplayName => "ObstacleController";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Enable", Direction.In),
				new ConnectionDesc("Disable", Direction.In)
			};
		}

		public ObstacleControllerEntity(FrostySdk.Ebx.ObstacleControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


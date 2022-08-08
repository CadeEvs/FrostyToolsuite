using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotStateLogicEntityData))]
	public class CinebotStateLogicEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CinebotStateLogicEntityData>
	{
		public new FrostySdk.Ebx.CinebotStateLogicEntityData Data => data as FrostySdk.Ebx.CinebotStateLogicEntityData;
		public override string DisplayName => "CinebotStateLogic";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Begin", Direction.In),
				new ConnectionDesc("End", Direction.In)
			};
		}

		public CinebotStateLogicEntity(FrostySdk.Ebx.CinebotStateLogicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


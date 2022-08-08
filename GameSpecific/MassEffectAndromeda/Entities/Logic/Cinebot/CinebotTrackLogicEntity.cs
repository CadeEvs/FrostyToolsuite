using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotTrackLogicEntityData))]
	public class CinebotTrackLogicEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CinebotTrackLogicEntityData>
	{
		public new FrostySdk.Ebx.CinebotTrackLogicEntityData Data => data as FrostySdk.Ebx.CinebotTrackLogicEntityData;
		public override string DisplayName => "CinebotTrackLogic";
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Begin", Direction.In)
			};
		}

		public CinebotTrackLogicEntity(FrostySdk.Ebx.CinebotTrackLogicEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


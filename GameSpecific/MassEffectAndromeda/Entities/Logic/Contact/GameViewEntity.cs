using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.GameViewEntityData))]
	public class GameViewEntity : LogicEntity, IEntityData<FrostySdk.Ebx.GameViewEntityData>
	{
		public new FrostySdk.Ebx.GameViewEntityData Data => data as FrostySdk.Ebx.GameViewEntityData;
		public override string DisplayName => "GameView";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("OnInit", Direction.Out)
			};
		}

		public GameViewEntity(FrostySdk.Ebx.GameViewEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PlayerFilterEntityData))]
	public class PlayerFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PlayerFilterEntityData>
	{
		public new FrostySdk.Ebx.PlayerFilterEntityData Data => data as FrostySdk.Ebx.PlayerFilterEntityData;
		public override string DisplayName => "PlayerFilter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Controllable", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Events
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("In", Direction.In)
			};
		}

		public PlayerFilterEntity(FrostySdk.Ebx.PlayerFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


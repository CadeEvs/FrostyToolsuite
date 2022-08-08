using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OnRevivedTeammateEntityData))]
	public class OnRevivedTeammateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.OnRevivedTeammateEntityData>
	{
		public new FrostySdk.Ebx.OnRevivedTeammateEntityData Data => data as FrostySdk.Ebx.OnRevivedTeammateEntityData;
		public override string DisplayName => "OnRevivedTeammate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("OnRevivedTeammate", Direction.Out)
			};
		}

        public OnRevivedTeammateEntity(FrostySdk.Ebx.OnRevivedTeammateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


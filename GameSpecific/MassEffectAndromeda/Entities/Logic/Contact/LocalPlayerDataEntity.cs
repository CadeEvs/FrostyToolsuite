using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalPlayerDataEntityData))]
	public class LocalPlayerDataEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocalPlayerDataEntityData>
	{
		public new FrostySdk.Ebx.LocalPlayerDataEntityData Data => data as FrostySdk.Ebx.LocalPlayerDataEntityData;
		public override string DisplayName => "LocalPlayerData";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Properties
        {
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("CameraTransform", Direction.Out),
				new ConnectionDesc("CharacterTransform", Direction.Out)
			};
        }

        public LocalPlayerDataEntity(FrostySdk.Ebx.LocalPlayerDataEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


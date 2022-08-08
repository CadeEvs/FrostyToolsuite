using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IsSinglePlayerEntityData))]
	public class IsSinglePlayerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IsSinglePlayerEntityData>
	{
		public new FrostySdk.Ebx.IsSinglePlayerEntityData Data => data as FrostySdk.Ebx.IsSinglePlayerEntityData;
		public override string DisplayName => "IsSinglePlayer";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public IsSinglePlayerEntity(FrostySdk.Ebx.IsSinglePlayerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


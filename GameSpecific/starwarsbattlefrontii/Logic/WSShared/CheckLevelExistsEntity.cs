using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CheckLevelExistsEntityData))]
	public class CheckLevelExistsEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CheckLevelExistsEntityData>
	{
		public new FrostySdk.Ebx.CheckLevelExistsEntityData Data => data as FrostySdk.Ebx.CheckLevelExistsEntityData;
		public override string DisplayName => "CheckLevelExists";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CheckLevelExistsEntity(FrostySdk.Ebx.CheckLevelExistsEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


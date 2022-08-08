using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BuildInfoEntityData))]
	public class BuildInfoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BuildInfoEntityData>
	{
		public new FrostySdk.Ebx.BuildInfoEntityData Data => data as FrostySdk.Ebx.BuildInfoEntityData;
		public override string DisplayName => "BuildInfo";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BuildInfoEntity(FrostySdk.Ebx.BuildInfoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


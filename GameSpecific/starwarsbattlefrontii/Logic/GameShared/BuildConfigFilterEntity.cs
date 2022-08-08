using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BuildConfigFilterEntityData))]
	public class BuildConfigFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.BuildConfigFilterEntityData>
	{
		public new FrostySdk.Ebx.BuildConfigFilterEntityData Data => data as FrostySdk.Ebx.BuildConfigFilterEntityData;
		public override string DisplayName => "BuildConfigFilter";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public BuildConfigFilterEntity(FrostySdk.Ebx.BuildConfigFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


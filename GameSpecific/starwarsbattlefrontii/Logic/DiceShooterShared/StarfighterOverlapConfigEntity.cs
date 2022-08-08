using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StarfighterOverlapConfigEntityData))]
	public class StarfighterOverlapConfigEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StarfighterOverlapConfigEntityData>
	{
		public new FrostySdk.Ebx.StarfighterOverlapConfigEntityData Data => data as FrostySdk.Ebx.StarfighterOverlapConfigEntityData;
		public override string DisplayName => "StarfighterOverlapConfig";

		public StarfighterOverlapConfigEntity(FrostySdk.Ebx.StarfighterOverlapConfigEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


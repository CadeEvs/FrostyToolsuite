using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StarfighterConfigEntityData))]
	public class StarfighterConfigEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StarfighterConfigEntityData>
	{
		public new FrostySdk.Ebx.StarfighterConfigEntityData Data => data as FrostySdk.Ebx.StarfighterConfigEntityData;
		public override string DisplayName => "StarfighterConfig";

		public StarfighterConfigEntity(FrostySdk.Ebx.StarfighterConfigEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


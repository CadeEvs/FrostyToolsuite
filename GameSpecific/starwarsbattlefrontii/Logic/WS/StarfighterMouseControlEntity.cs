using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StarfighterMouseControlEntityData))]
	public class StarfighterMouseControlEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StarfighterMouseControlEntityData>
	{
		public new FrostySdk.Ebx.StarfighterMouseControlEntityData Data => data as FrostySdk.Ebx.StarfighterMouseControlEntityData;
		public override string DisplayName => "StarfighterMouseControl";

		public StarfighterMouseControlEntity(FrostySdk.Ebx.StarfighterMouseControlEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


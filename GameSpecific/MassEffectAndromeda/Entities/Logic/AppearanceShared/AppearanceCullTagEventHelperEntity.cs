using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AppearanceCullTagEventHelperEntityData))]
	public class AppearanceCullTagEventHelperEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AppearanceCullTagEventHelperEntityData>
	{
		public new FrostySdk.Ebx.AppearanceCullTagEventHelperEntityData Data => data as FrostySdk.Ebx.AppearanceCullTagEventHelperEntityData;
		public override string DisplayName => "AppearanceCullTagEventHelper";

		public AppearanceCullTagEventHelperEntity(FrostySdk.Ebx.AppearanceCullTagEventHelperEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


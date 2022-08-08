using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSTutorialEntityData))]
	public class WSTutorialEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSTutorialEntityData>
	{
		public new FrostySdk.Ebx.WSTutorialEntityData Data => data as FrostySdk.Ebx.WSTutorialEntityData;
		public override string DisplayName => "WSTutorial";

		public WSTutorialEntity(FrostySdk.Ebx.WSTutorialEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TutorialDataProviderData))]
	public class TutorialDataProvider : LogicEntity, IEntityData<FrostySdk.Ebx.TutorialDataProviderData>
	{
		public new FrostySdk.Ebx.TutorialDataProviderData Data => data as FrostySdk.Ebx.TutorialDataProviderData;
		public override string DisplayName => "TutorialDataProvider";

		public TutorialDataProvider(FrostySdk.Ebx.TutorialDataProviderData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


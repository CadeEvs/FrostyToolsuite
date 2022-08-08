using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIEntityTrackControllerEntityData))]
	public class UIEntityTrackControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.UIEntityTrackControllerEntityData>
	{
		public new FrostySdk.Ebx.UIEntityTrackControllerEntityData Data => data as FrostySdk.Ebx.UIEntityTrackControllerEntityData;
		public override string DisplayName => "UIEntityTrackController";

		public UIEntityTrackControllerEntity(FrostySdk.Ebx.UIEntityTrackControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


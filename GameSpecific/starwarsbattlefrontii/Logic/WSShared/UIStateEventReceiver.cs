using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIStateEventReceiverData))]
	public class UIStateEventReceiver : LogicEntity, IEntityData<FrostySdk.Ebx.UIStateEventReceiverData>
	{
		public new FrostySdk.Ebx.UIStateEventReceiverData Data => data as FrostySdk.Ebx.UIStateEventReceiverData;
		public override string DisplayName => "UIStateEventReceiver";

		public UIStateEventReceiver(FrostySdk.Ebx.UIStateEventReceiverData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


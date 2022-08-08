using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWGetTransformFromPlayerEventData))]
	public class BWGetTransformFromPlayerEvent : LogicEntity, IEntityData<FrostySdk.Ebx.BWGetTransformFromPlayerEventData>
	{
		public new FrostySdk.Ebx.BWGetTransformFromPlayerEventData Data => data as FrostySdk.Ebx.BWGetTransformFromPlayerEventData;
		public override string DisplayName => "BWGetTransformFromPlayerEvent";

		public BWGetTransformFromPlayerEvent(FrostySdk.Ebx.BWGetTransformFromPlayerEventData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


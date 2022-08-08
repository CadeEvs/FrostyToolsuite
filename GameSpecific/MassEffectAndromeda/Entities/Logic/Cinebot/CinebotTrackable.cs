using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CinebotTrackableData))]
	public class CinebotTrackable : LogicEntity, IEntityData<FrostySdk.Ebx.CinebotTrackableData>
	{
		public new FrostySdk.Ebx.CinebotTrackableData Data => data as FrostySdk.Ebx.CinebotTrackableData;
		public override string DisplayName => "CinebotTrackable";

		public CinebotTrackable(FrostySdk.Ebx.CinebotTrackableData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


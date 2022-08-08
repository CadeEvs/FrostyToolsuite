using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ControllerLookAtActionEntityData))]
	public class ControllerLookAtActionEntity : ControllerSpecificActionEntity, IEntityData<FrostySdk.Ebx.ControllerLookAtActionEntityData>
	{
		public new FrostySdk.Ebx.ControllerLookAtActionEntityData Data => data as FrostySdk.Ebx.ControllerLookAtActionEntityData;
		public override string DisplayName => "ControllerLookAtAction";

		public ControllerLookAtActionEntity(FrostySdk.Ebx.ControllerLookAtActionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidAttachEntityData))]
	public class DroidAttachEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DroidAttachEntityData>
	{
		public new FrostySdk.Ebx.DroidAttachEntityData Data => data as FrostySdk.Ebx.DroidAttachEntityData;
		public override string DisplayName => "DroidAttach";

		public DroidAttachEntity(FrostySdk.Ebx.DroidAttachEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


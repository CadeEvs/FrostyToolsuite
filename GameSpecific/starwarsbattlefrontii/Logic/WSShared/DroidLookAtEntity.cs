using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DroidLookAtEntityData))]
	public class DroidLookAtEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DroidLookAtEntityData>
	{
		public new FrostySdk.Ebx.DroidLookAtEntityData Data => data as FrostySdk.Ebx.DroidLookAtEntityData;
		public override string DisplayName => "DroidLookAt";

		public DroidLookAtEntity(FrostySdk.Ebx.DroidLookAtEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SaveEntityData))]
	public class SaveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SaveEntityData>
	{
		public new FrostySdk.Ebx.SaveEntityData Data => data as FrostySdk.Ebx.SaveEntityData;
		public override string DisplayName => "Save";

		public SaveEntity(FrostySdk.Ebx.SaveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


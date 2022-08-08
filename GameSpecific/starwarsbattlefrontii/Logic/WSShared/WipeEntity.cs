using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WipeEntityData))]
	public class WipeEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WipeEntityData>
	{
		public new FrostySdk.Ebx.WipeEntityData Data => data as FrostySdk.Ebx.WipeEntityData;
		public override string DisplayName => "Wipe";

		public WipeEntity(FrostySdk.Ebx.WipeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


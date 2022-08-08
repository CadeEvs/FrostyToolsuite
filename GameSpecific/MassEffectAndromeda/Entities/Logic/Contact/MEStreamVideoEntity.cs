using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEStreamVideoEntityData))]
	public class MEStreamVideoEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEStreamVideoEntityData>
	{
		public new FrostySdk.Ebx.MEStreamVideoEntityData Data => data as FrostySdk.Ebx.MEStreamVideoEntityData;
		public override string DisplayName => "MEStreamVideo";

		public MEStreamVideoEntity(FrostySdk.Ebx.MEStreamVideoEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SimpleEntityData))]
	public class SimpleEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SimpleEntityData>
	{
		public new FrostySdk.Ebx.SimpleEntityData Data => data as FrostySdk.Ebx.SimpleEntityData;
		public override string DisplayName => "Simple";

		public SimpleEntity(FrostySdk.Ebx.SimpleEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


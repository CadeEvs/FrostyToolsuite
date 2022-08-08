using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StartPointEntityData))]
	public class StartPointEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StartPointEntityData>
	{
		public new FrostySdk.Ebx.StartPointEntityData Data => data as FrostySdk.Ebx.StartPointEntityData;
		public override string DisplayName => "StartPoint";

		public StartPointEntity(FrostySdk.Ebx.StartPointEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


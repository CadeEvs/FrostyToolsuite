using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScatterContentManagerEntityData))]
	public class ScatterContentManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScatterContentManagerEntityData>
	{
		public new FrostySdk.Ebx.ScatterContentManagerEntityData Data => data as FrostySdk.Ebx.ScatterContentManagerEntityData;
		public override string DisplayName => "ScatterContentManager";

		public ScatterContentManagerEntity(FrostySdk.Ebx.ScatterContentManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


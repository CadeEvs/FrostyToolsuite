using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ScatterContentManagerControllerEntityData))]
	public class ScatterContentManagerControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ScatterContentManagerControllerEntityData>
	{
		public new FrostySdk.Ebx.ScatterContentManagerControllerEntityData Data => data as FrostySdk.Ebx.ScatterContentManagerControllerEntityData;
		public override string DisplayName => "ScatterContentManagerController";

		public ScatterContentManagerControllerEntity(FrostySdk.Ebx.ScatterContentManagerControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


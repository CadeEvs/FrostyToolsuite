using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SensingManagerEntityData))]
	public class SensingManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SensingManagerEntityData>
	{
		public new FrostySdk.Ebx.SensingManagerEntityData Data => data as FrostySdk.Ebx.SensingManagerEntityData;
		public override string DisplayName => "SensingManager";

		public SensingManagerEntity(FrostySdk.Ebx.SensingManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


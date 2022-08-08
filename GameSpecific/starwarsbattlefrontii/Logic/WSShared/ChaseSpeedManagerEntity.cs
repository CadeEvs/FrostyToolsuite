using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ChaseSpeedManagerEntityData))]
	public class ChaseSpeedManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ChaseSpeedManagerEntityData>
	{
		public new FrostySdk.Ebx.ChaseSpeedManagerEntityData Data => data as FrostySdk.Ebx.ChaseSpeedManagerEntityData;
		public override string DisplayName => "ChaseSpeedManager";

		public ChaseSpeedManagerEntity(FrostySdk.Ebx.ChaseSpeedManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


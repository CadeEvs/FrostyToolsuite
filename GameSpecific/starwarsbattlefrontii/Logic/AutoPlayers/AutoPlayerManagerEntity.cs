using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AutoPlayerManagerEntityData))]
	public class AutoPlayerManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.AutoPlayerManagerEntityData>
	{
		public new FrostySdk.Ebx.AutoPlayerManagerEntityData Data => data as FrostySdk.Ebx.AutoPlayerManagerEntityData;
		public override string DisplayName => "AutoPlayerManager";

		public AutoPlayerManagerEntity(FrostySdk.Ebx.AutoPlayerManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


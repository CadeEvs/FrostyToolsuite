using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetEntityDirectiveEntityData))]
	public class SetEntityDirectiveEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetEntityDirectiveEntityData>
	{
		public new FrostySdk.Ebx.SetEntityDirectiveEntityData Data => data as FrostySdk.Ebx.SetEntityDirectiveEntityData;
		public override string DisplayName => "SetEntityDirective";

		public SetEntityDirectiveEntity(FrostySdk.Ebx.SetEntityDirectiveEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputRestrictionEntityData))]
	public class InputRestrictionEntity : LogicEntity, IEntityData<FrostySdk.Ebx.InputRestrictionEntityData>
	{
		public new FrostySdk.Ebx.InputRestrictionEntityData Data => data as FrostySdk.Ebx.InputRestrictionEntityData;
		public override string DisplayName => "InputRestriction";

		public InputRestrictionEntity(FrostySdk.Ebx.InputRestrictionEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


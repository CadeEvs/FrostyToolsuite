using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputQueryEntityData))]
	public class InputQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.InputQueryEntityData>
	{
		public new FrostySdk.Ebx.InputQueryEntityData Data => data as FrostySdk.Ebx.InputQueryEntityData;
		public override string DisplayName => "InputQuery";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public InputQueryEntity(FrostySdk.Ebx.InputQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


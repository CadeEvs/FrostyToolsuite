using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputProgressEntityData))]
	public class InputProgressEntity : LogicEntity, IEntityData<FrostySdk.Ebx.InputProgressEntityData>
	{
		public new FrostySdk.Ebx.InputProgressEntityData Data => data as FrostySdk.Ebx.InputProgressEntityData;
		public override string DisplayName => "InputProgress";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public InputProgressEntity(FrostySdk.Ebx.InputProgressEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


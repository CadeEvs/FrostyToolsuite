using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ControlInputEntityData))]
	public class ControlInputEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ControlInputEntityData>
	{
		public new FrostySdk.Ebx.ControlInputEntityData Data => data as FrostySdk.Ebx.ControlInputEntityData;
		public override string DisplayName => "ControlInput";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public ControlInputEntity(FrostySdk.Ebx.ControlInputEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


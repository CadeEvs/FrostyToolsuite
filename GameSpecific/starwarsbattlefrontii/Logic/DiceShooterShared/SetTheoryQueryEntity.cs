using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SetTheoryQueryEntityData))]
	public class SetTheoryQueryEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SetTheoryQueryEntityData>
	{
		public new FrostySdk.Ebx.SetTheoryQueryEntityData Data => data as FrostySdk.Ebx.SetTheoryQueryEntityData;
		public override string DisplayName => "SetTheoryQuery";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SetTheoryQueryEntity(FrostySdk.Ebx.SetTheoryQueryEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


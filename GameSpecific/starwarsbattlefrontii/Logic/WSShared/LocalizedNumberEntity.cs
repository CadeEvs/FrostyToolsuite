using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LocalizedNumberEntityData))]
	public class LocalizedNumberEntity : LogicEntity, IEntityData<FrostySdk.Ebx.LocalizedNumberEntityData>
	{
		public new FrostySdk.Ebx.LocalizedNumberEntityData Data => data as FrostySdk.Ebx.LocalizedNumberEntityData;
		public override string DisplayName => "LocalizedNumber";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public LocalizedNumberEntity(FrostySdk.Ebx.LocalizedNumberEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


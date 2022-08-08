using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WatergateEntityData))]
	public class WatergateEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WatergateEntityData>
	{
		public new FrostySdk.Ebx.WatergateEntityData Data => data as FrostySdk.Ebx.WatergateEntityData;
		public override string DisplayName => "Watergate";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WatergateEntity(FrostySdk.Ebx.WatergateEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


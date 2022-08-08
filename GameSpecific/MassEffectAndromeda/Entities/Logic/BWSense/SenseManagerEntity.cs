using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SenseManagerEntityData))]
	public class SenseManagerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SenseManagerEntityData>
	{
		public new FrostySdk.Ebx.SenseManagerEntityData Data => data as FrostySdk.Ebx.SenseManagerEntityData;
		public override string DisplayName => "SenseManager";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SenseManagerEntity(FrostySdk.Ebx.SenseManagerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


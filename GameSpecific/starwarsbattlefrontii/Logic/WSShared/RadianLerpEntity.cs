using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.RadianLerpEntityData))]
	public class RadianLerpEntity : LogicEntity, IEntityData<FrostySdk.Ebx.RadianLerpEntityData>
	{
		public new FrostySdk.Ebx.RadianLerpEntityData Data => data as FrostySdk.Ebx.RadianLerpEntityData;
		public override string DisplayName => "RadianLerp";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public RadianLerpEntity(FrostySdk.Ebx.RadianLerpEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


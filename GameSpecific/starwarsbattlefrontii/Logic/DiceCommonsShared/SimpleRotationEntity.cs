using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SimpleRotationEntityData))]
	public class SimpleRotationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SimpleRotationEntityData>
	{
		public new FrostySdk.Ebx.SimpleRotationEntityData Data => data as FrostySdk.Ebx.SimpleRotationEntityData;
		public override string DisplayName => "SimpleRotation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public SimpleRotationEntity(FrostySdk.Ebx.SimpleRotationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


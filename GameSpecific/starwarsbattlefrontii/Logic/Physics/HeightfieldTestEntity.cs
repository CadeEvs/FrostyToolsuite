using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.HeightfieldTestEntityData))]
	public class HeightfieldTestEntity : LogicEntity, IEntityData<FrostySdk.Ebx.HeightfieldTestEntityData>
	{
		public new FrostySdk.Ebx.HeightfieldTestEntityData Data => data as FrostySdk.Ebx.HeightfieldTestEntityData;
		public override string DisplayName => "HeightfieldTest";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public HeightfieldTestEntity(FrostySdk.Ebx.HeightfieldTestEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


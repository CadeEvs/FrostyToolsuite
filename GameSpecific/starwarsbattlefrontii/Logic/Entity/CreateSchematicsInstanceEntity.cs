using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreateSchematicsInstanceEntityData))]
	public class CreateSchematicsInstanceEntity : LogicEntity, IEntityData<FrostySdk.Ebx.CreateSchematicsInstanceEntityData>
	{
		public new FrostySdk.Ebx.CreateSchematicsInstanceEntityData Data => data as FrostySdk.Ebx.CreateSchematicsInstanceEntityData;
		public override string DisplayName => "CreateSchematicsInstance";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public CreateSchematicsInstanceEntity(FrostySdk.Ebx.CreateSchematicsInstanceEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


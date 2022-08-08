using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.JointValidationEntityData))]
	public class JointValidationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.JointValidationEntityData>
	{
		public new FrostySdk.Ebx.JointValidationEntityData Data => data as FrostySdk.Ebx.JointValidationEntityData;
		public override string DisplayName => "JointValidation";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public JointValidationEntity(FrostySdk.Ebx.JointValidationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


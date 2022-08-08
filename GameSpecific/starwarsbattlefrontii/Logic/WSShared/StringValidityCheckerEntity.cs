using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.StringValidityCheckerEntityData))]
	public class StringValidityCheckerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.StringValidityCheckerEntityData>
	{
		public new FrostySdk.Ebx.StringValidityCheckerEntityData Data => data as FrostySdk.Ebx.StringValidityCheckerEntityData;
		public override string DisplayName => "StringValidityChecker";

		public StringValidityCheckerEntity(FrostySdk.Ebx.StringValidityCheckerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


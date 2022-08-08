using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SensingAreaModifierEntityData))]
	public class SensingAreaModifierEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.SensingAreaModifierEntityData>
	{
		public new FrostySdk.Ebx.SensingAreaModifierEntityData Data => data as FrostySdk.Ebx.SensingAreaModifierEntityData;
		public override string DisplayName => "SensingAreaModifier";

		public SensingAreaModifierEntity(FrostySdk.Ebx.SensingAreaModifierEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


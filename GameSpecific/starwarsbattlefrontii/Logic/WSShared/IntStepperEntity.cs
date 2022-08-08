using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntStepperEntityData))]
	public class IntStepperEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IntStepperEntityData>
	{
		public new FrostySdk.Ebx.IntStepperEntityData Data => data as FrostySdk.Ebx.IntStepperEntityData;
		public override string DisplayName => "IntStepper";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public IntStepperEntity(FrostySdk.Ebx.IntStepperEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


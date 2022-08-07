using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IntegratorOrDifferentiatorEntityData))]
	public class IntegratorOrDifferentiatorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IntegratorOrDifferentiatorEntityData>
	{
		public new FrostySdk.Ebx.IntegratorOrDifferentiatorEntityData Data => data as FrostySdk.Ebx.IntegratorOrDifferentiatorEntityData;
		public override string DisplayName => "IntegratorOrDifferentiator";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public IntegratorOrDifferentiatorEntity(FrostySdk.Ebx.IntegratorOrDifferentiatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSIntegratorOrDifferentiatorEntityData))]
	public class WSIntegratorOrDifferentiatorEntity : LogicEntity, IEntityData<FrostySdk.Ebx.WSIntegratorOrDifferentiatorEntityData>
	{
		public new FrostySdk.Ebx.WSIntegratorOrDifferentiatorEntityData Data => data as FrostySdk.Ebx.WSIntegratorOrDifferentiatorEntityData;
		public override string DisplayName => "WSIntegratorOrDifferentiator";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public WSIntegratorOrDifferentiatorEntity(FrostySdk.Ebx.WSIntegratorOrDifferentiatorEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


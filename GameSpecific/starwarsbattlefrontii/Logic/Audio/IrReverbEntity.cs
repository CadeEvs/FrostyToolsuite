using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.IrReverbEntityData))]
	public class IrReverbEntity : LogicEntity, IEntityData<FrostySdk.Ebx.IrReverbEntityData>
	{
		public new FrostySdk.Ebx.IrReverbEntityData Data => data as FrostySdk.Ebx.IrReverbEntityData;
		public override string DisplayName => "IrReverb";

		public IrReverbEntity(FrostySdk.Ebx.IrReverbEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


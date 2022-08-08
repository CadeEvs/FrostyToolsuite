using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.DiceSoundAreaEntityData))]
	public class DiceSoundAreaEntity : LogicEntity, IEntityData<FrostySdk.Ebx.DiceSoundAreaEntityData>
	{
		public new FrostySdk.Ebx.DiceSoundAreaEntityData Data => data as FrostySdk.Ebx.DiceSoundAreaEntityData;
		public override string DisplayName => "DiceSoundArea";

		public DiceSoundAreaEntity(FrostySdk.Ebx.DiceSoundAreaEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


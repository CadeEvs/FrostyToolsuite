using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MedigelUIData))]
	public class MedigelUI : LogicEntity, IEntityData<FrostySdk.Ebx.MedigelUIData>
	{
		public new FrostySdk.Ebx.MedigelUIData Data => data as FrostySdk.Ebx.MedigelUIData;
		public override string DisplayName => "MedigelUI";

		public MedigelUI(FrostySdk.Ebx.MedigelUIData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


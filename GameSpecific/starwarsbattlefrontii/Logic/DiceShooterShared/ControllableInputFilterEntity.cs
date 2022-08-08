using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ControllableInputFilterEntityData))]
	public class ControllableInputFilterEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ControllableInputFilterEntityData>
	{
		public new FrostySdk.Ebx.ControllableInputFilterEntityData Data => data as FrostySdk.Ebx.ControllableInputFilterEntityData;
		public override string DisplayName => "ControllableInputFilter";

		public ControllableInputFilterEntity(FrostySdk.Ebx.ControllableInputFilterEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


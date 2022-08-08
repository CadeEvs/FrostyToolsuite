using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.CreateMatchUIData))]
	public class CreateMatchUI : LogicEntity, IEntityData<FrostySdk.Ebx.CreateMatchUIData>
	{
		public new FrostySdk.Ebx.CreateMatchUIData Data => data as FrostySdk.Ebx.CreateMatchUIData;
		public override string DisplayName => "CreateMatchUI";

		public CreateMatchUI(FrostySdk.Ebx.CreateMatchUIData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEUIPressStartEntityData))]
	public class MEUIPressStartEntity : LogicEntity, IEntityData<FrostySdk.Ebx.MEUIPressStartEntityData>
	{
		public new FrostySdk.Ebx.MEUIPressStartEntityData Data => data as FrostySdk.Ebx.MEUIPressStartEntityData;
		public override string DisplayName => "MEUIPressStart";

		public MEUIPressStartEntity(FrostySdk.Ebx.MEUIPressStartEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


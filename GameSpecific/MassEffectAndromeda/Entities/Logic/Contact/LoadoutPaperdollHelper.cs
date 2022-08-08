using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.LoadoutPaperdollHelperData))]
	public class LoadoutPaperdollHelper : LogicEntity, IEntityData<FrostySdk.Ebx.LoadoutPaperdollHelperData>
	{
		public new FrostySdk.Ebx.LoadoutPaperdollHelperData Data => data as FrostySdk.Ebx.LoadoutPaperdollHelperData;
		public override string DisplayName => "LoadoutPaperdollHelper";

		public LoadoutPaperdollHelper(FrostySdk.Ebx.LoadoutPaperdollHelperData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.MEBangerEntityData))]
	public class MEBangerEntity : BangerEntity, IEntityData<FrostySdk.Ebx.MEBangerEntityData>
	{
		public new FrostySdk.Ebx.MEBangerEntityData Data => data as FrostySdk.Ebx.MEBangerEntityData;

		public MEBangerEntity(FrostySdk.Ebx.MEBangerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


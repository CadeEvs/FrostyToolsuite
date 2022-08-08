using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.WSGrenadeEntityData))]
	public class WSGrenadeEntity : GrenadeEntity, IEntityData<FrostySdk.Ebx.WSGrenadeEntityData>
	{
		public new FrostySdk.Ebx.WSGrenadeEntityData Data => data as FrostySdk.Ebx.WSGrenadeEntityData;

		public WSGrenadeEntity(FrostySdk.Ebx.WSGrenadeEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


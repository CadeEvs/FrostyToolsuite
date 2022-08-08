using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BangerEntityData))]
	public class BangerEntity : DynamicGamePhysicsEntity, IEntityData<FrostySdk.Ebx.BangerEntityData>
	{
		public new FrostySdk.Ebx.BangerEntityData Data => data as FrostySdk.Ebx.BangerEntityData;

		public BangerEntity(FrostySdk.Ebx.BangerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


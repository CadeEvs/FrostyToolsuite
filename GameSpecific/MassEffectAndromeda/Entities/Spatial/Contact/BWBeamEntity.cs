using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.BWBeamEntityData))]
	public class BWBeamEntity : GamePhysicsEntity, IEntityData<FrostySdk.Ebx.BWBeamEntityData>
	{
		public new FrostySdk.Ebx.BWBeamEntityData Data => data as FrostySdk.Ebx.BWBeamEntityData;

		public BWBeamEntity(FrostySdk.Ebx.BWBeamEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


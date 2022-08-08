using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.AISelfDestructEntityData))]
	public class AISelfDestructEntity : AIParameterEntity, IEntityData<FrostySdk.Ebx.AISelfDestructEntityData>
	{
		public new FrostySdk.Ebx.AISelfDestructEntityData Data => data as FrostySdk.Ebx.AISelfDestructEntityData;
		public override string DisplayName => "AISelfDestruct";

		public AISelfDestructEntity(FrostySdk.Ebx.AISelfDestructEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.TripwireEntityData))]
	public class TripwireEntity : GameComponentEntity, IEntityData<FrostySdk.Ebx.TripwireEntityData>
	{
		public new FrostySdk.Ebx.TripwireEntityData Data => data as FrostySdk.Ebx.TripwireEntityData;

		public TripwireEntity(FrostySdk.Ebx.TripwireEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


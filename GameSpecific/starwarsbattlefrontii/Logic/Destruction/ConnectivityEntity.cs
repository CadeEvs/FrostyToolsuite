using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.ConnectivityEntityData))]
	public class ConnectivityEntity : LogicEntity, IEntityData<FrostySdk.Ebx.ConnectivityEntityData>
	{
		public new FrostySdk.Ebx.ConnectivityEntityData Data => data as FrostySdk.Ebx.ConnectivityEntityData;
		public override string DisplayName => "Connectivity";

		public ConnectivityEntity(FrostySdk.Ebx.ConnectivityEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


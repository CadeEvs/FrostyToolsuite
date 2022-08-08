using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputToNetworkedBoolEntityData))]
	public class InputToNetworkedBoolEntity : LogicEntity, IEntityData<FrostySdk.Ebx.InputToNetworkedBoolEntityData>
	{
		public new FrostySdk.Ebx.InputToNetworkedBoolEntityData Data => data as FrostySdk.Ebx.InputToNetworkedBoolEntityData;
		public override string DisplayName => "InputToNetworkedBool";

		public InputToNetworkedBoolEntity(FrostySdk.Ebx.InputToNetworkedBoolEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


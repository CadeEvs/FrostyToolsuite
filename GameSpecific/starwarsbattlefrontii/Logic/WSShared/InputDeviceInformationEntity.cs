using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.InputDeviceInformationEntityData))]
	public class InputDeviceInformationEntity : LogicEntity, IEntityData<FrostySdk.Ebx.InputDeviceInformationEntityData>
	{
		public new FrostySdk.Ebx.InputDeviceInformationEntityData Data => data as FrostySdk.Ebx.InputDeviceInformationEntityData;
		public override string DisplayName => "InputDeviceInformation";

		public InputDeviceInformationEntity(FrostySdk.Ebx.InputDeviceInformationEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}


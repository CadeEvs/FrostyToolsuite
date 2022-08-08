using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.NarrativeNPCControllerEntityData))]
	public class NarrativeNPCControllerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.NarrativeNPCControllerEntityData>
	{
		public new FrostySdk.Ebx.NarrativeNPCControllerEntityData Data => data as FrostySdk.Ebx.NarrativeNPCControllerEntityData;
		public override string DisplayName => "NarrativeNPCController";

		public NarrativeNPCControllerEntity(FrostySdk.Ebx.NarrativeNPCControllerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

